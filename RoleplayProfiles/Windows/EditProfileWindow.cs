using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using RoleplayProfiles.Api;
using RoleplayProfiles.State;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace RoleplayProfiles.Windows;

public class EditProfileWindow : Window, IDisposable
{
    private static readonly string DefaultTitle = "Edit profile###EditProfile";
    private static readonly float LabelWidth = ImGuiHelpers.ScaledVector2(100).X;

    private readonly PluginState pluginState;

    private Profile? profile = null;
    private LocalProfile? localProfile = null;
    private volatile string errorMessage = " ";
    private volatile bool saving = false;

    public EditProfileWindow(PluginState pluginState) : base(DefaultTitle)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 520),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.pluginState = pluginState;
    }

    public void Dispose()
    {
        profile = null;
        localProfile = null;
        errorMessage = " ";
        saving = false;
    }

    public override void Draw()
    {
        var player = pluginState.GetCurrentPlayer();

        if (player == null || pluginState.Configuration.AccessToken == null)
        {
            this.WindowName = DefaultTitle;
            return;
        }

        this.WindowName = $"Edit profile: {player.Name}###EditProfile";

        var cacheEntry = pluginState.GetProfile(player);

        if (cacheEntry.State == CacheEntryState.Pending)
        {
            ImGui.TextColored(Colors.Loading, "Retrieving profile...");
            return;
        }
        else if (cacheEntry.State == CacheEntryState.NotFound)
        {
            ImGui.TextWrapped($"Profile not found. It is possible that you have not added this character " +
                    $"{player.Name} ({player.Server}) to your Chaos Archives account, or have not yet verified " +
                    $"your ownership of this character.");

            if (ImGui.Button("Open Chaos Archives"))
            {
                Process.Start(new ProcessStartInfo { FileName = "https://chaosarchives.org", UseShellExecute = true });
            }

            return;
        }
        
        // case CacheEntryState.Retrieved:
        // Profile instances are never modified once created, so != is safe
        if (cacheEntry.Data != profile || localProfile == null)
        {
            profile = cacheEntry.Data;
            localProfile = LocalProfile.Of(profile!);
        }

        // Editing area
        ImGui.BeginChild("ScrollRegion", ImGuiHelpers.ScaledVector2(0, -32));
        ImGui.PushItemWidth(-10);

        ImGui.Text("At first glance");
        FieldRow("Title", ref localProfile.title);
        FieldRow("Nickname", ref localProfile.nickname);
        FieldRow("Occupation", ref localProfile.occupation);
        FieldRow("Pronouns", ref localProfile.pronouns, 20);
        FieldRow("Currently", ref localProfile.currently, 1000);
        FieldRow("OOC Info", ref localProfile.oocInfo, 1000);

        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(6);
        ImGui.Text("Expanded profile");

        ImGui.PushStyleColor(ImGuiCol.Text, Colors.OocInfo);
        ImGui.TextWrapped("To edit outward appearance and background, use the Chaos Archives website.");
        ImGui.PopStyleColor();

        FieldRow("Age", ref localProfile.age);
        FieldRow("Birthplace", ref localProfile.birthplace);
        FieldRow("Residence", ref localProfile.residence);
        FieldRow("Friends", ref localProfile.friends, 1000);
        FieldRow("Relatives", ref localProfile.relatives, 1000);
        FieldRow("Rivals/Enemies", ref localProfile.enemies, 1000);
        FieldRow("Loves", ref localProfile.loves, 1000);
        FieldRow("Hates", ref localProfile.hates, 1000);
        FieldRow("Motto", ref localProfile.motto, 1000);
        FieldRow("Motivation", ref localProfile.motivation, 1000);

        ImGuiHelpers.ScaledDummy(3);

        if (ImGui.Button("Open Chaos Archives profile"))
        {
            var url = $"https://chaosarchives.org/{player.Server}/{player.Name.Replace(' ', '_')}";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        ImGui.PopItemWidth();
        ImGui.EndChild();

        // Button bar
        var resetButtonText = "Reset";
        var resetButtonSize = ImGuiHelpers.GetButtonSize(resetButtonText);
        var saveButtonText = saving ? "Saving..." : "Save changes";
        var saveButtonSize = ImGuiHelpers.GetButtonSize(saveButtonText);

        ImGui.TextColored(Colors.Error, errorMessage);
        ImGui.SameLine(ImGui.GetWindowWidth() - resetButtonSize.X - saveButtonSize.X - ImGuiHelpers.ScaledVector2(16, 0).X);

        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);

        if (ImGui.Button(resetButtonText))
        {
            localProfile = LocalProfile.Of(profile!);
            errorMessage = " ";
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();
        ImGuiHelpers.ScaledDummy(0);
        ImGui.SameLine();

        if (ImGui.Button(saveButtonText))
        {
            _ = Save(player);
        }
    }

    private void FieldRow(string label, ref string field)
    {
        FieldRow(label, ref field, 255);
    }

    private void FieldRow(string label, ref string field, uint maxLength)
    {
        ImGui.TextColored(Colors.Label, label);
        ImGui.SameLine(LabelWidth);
        ImGui.InputText($" ###{label}", ref field, maxLength);
    }

    private async Task Save(Player player)
    {
        saving = true;

        try
        {
            await pluginState.SaveProfile(player, localProfile!.ToProfile());
            errorMessage = "Save successful";
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        }
        finally
        {
            saving = false;
        }
    }

    private class LocalProfile
    {
        public string title = "";

        public string nickname = "";
        public string occupation = "";
        public string currently = "";
        public string oocInfo = "";
        public string pronouns = "";

        public string age = "";
        public string birthplace = "";
        public string residence = "";
        public string friends = "";
        public string relatives = "";
        public string enemies = "";
        public string loves = "";
        public string hates = "";
        public string motto = "";
        public string motivation = "";

        // Read only fields
        private string appearance = "";
        private string background = "";
        private string race = "";

        public static LocalProfile Of(Profile profile)
        {
            return new LocalProfile
            {
                title = profile.Title,
                nickname = profile.Nickname,
                occupation = profile.Occupation,
                currently = profile.Currently,
                oocInfo = profile.OocInfo,
                pronouns = profile.Pronouns,
                age = profile.Age,
                birthplace = profile.Birthplace,
                residence = profile.Residence,
                friends = profile.Friends,
                relatives = profile.Relatives,
                enemies = profile.Enemies,
                loves = profile.Loves,
                hates = profile.Hates,
                motto = profile.Motto,
                motivation = profile.Motivation,
                appearance = profile.Appearance,
                background = profile.Background,
                race = profile.Race,
            };
        }

        public Profile ToProfile()
        {
            return new Profile
            {
                Title = title,
                Nickname = nickname,
                Occupation = occupation,
                Currently = currently,
                OocInfo = oocInfo,
                Pronouns = pronouns,
                Age = age,
                Birthplace = birthplace,
                Residence = residence,
                Friends = friends,
                Relatives = relatives,
                Enemies = enemies,
                Loves = loves,
                Hates = hates,
                Motto = motto,
                Motivation = motivation,
                Appearance = appearance,
                Background = background,
                Race = race,
            };
        }
    }
}
