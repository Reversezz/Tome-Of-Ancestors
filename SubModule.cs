using Bannerlord.UIExtenderEx;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

// v1.4.6 - v1.4.8
namespace TomeOfAncestors
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);                       
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            UIExtender extender = UIExtender.Create("TomeOfAncestors");
            extender.Register(typeof(SubModule).Assembly);
            extender.Enable();            // dsda
        }
    }
}