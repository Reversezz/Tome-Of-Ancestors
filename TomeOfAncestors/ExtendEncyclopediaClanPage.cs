using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Library;
using TaleWorlds.Localization;


namespace TomeOfAncestors
{
    // Расширяемый XPath (xml файл из Sandbox/Prefabs/...) и тег, с которым будем работать
    [PrefabExtension("EncyclopediaClanPage", "descendant::ListPanel[@Id='Leader']")] 
    public sealed class DeadMembersPatch : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append; 

        [PrefabExtensionFileName(true)] public string FileNamePatch => "DeadMembersPatch";
    }

    // Изменяем динамически расстояние между моим дивидером и оригинальным
    [PrefabExtension("EncyclopediaClanPage", "descendant::EncyclopediaDivider[@Id='MembersDivider']")]
    public class MembersDividerMarginPatch : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new List<Attribute>()
        {
            new Attribute("MarginTop", "20")
        };
    }   

    // По факту перехватываем выполнение метода RefreshValues
    // из EncyclopediaClanPageVM и добавляем туда собственную реализацию - DeadMembersPatch.
    // После перехвата мы вызываем наш патч, там наша dll сначала вылавливает FileNamePatch, а после 
    // тип внедрения и получает Append, а значит "добавление ПОСЛЕ выбранного тега.ы
    [ViewModelMixin("RefreshValues")] 
    public class ExtendEncyclopediaClanPageVM : BaseViewModelMixin<EncyclopediaClanPageVM>
    {
        // <EncyclopediaDivider ... Parameter.Title="@DeadMembersText" ... />
        [DataSourceProperty] public string DeadMembersText { get; set; }

        // <NavigatableGridWidget ... DataSource="{DeadMembers}" ... />
        [DataSourceProperty] public MBBindingList<HeroVM> DeadMembers { get; set; } 


        public ExtendEncyclopediaClanPageVM(EncyclopediaClanPageVM vm) : base(vm)
        {
            DeadMembersText = string.Empty;
            DeadMembers = new MBBindingList<HeroVM>();
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            DeadMembersText = new TextObject("{=ku8UIHh7}Dead clan members").ToString();
            DeadMembers.Clear();
            
            var members = Traverse.Create(ViewModel).Field("_members").GetValue<MBBindingList<HeroVM>>();

            var deadMembers = members.Where(m => m != null && m.IsDead).ToList();

            foreach (var deadMember in deadMembers)
            {
                if (deadMember != null && deadMember.IsDead)
                {
                    DeadMembers.Add(deadMember);
                    members.Remove(deadMember);
                }
            }
        }
    }
}