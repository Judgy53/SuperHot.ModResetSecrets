using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mod
{
    public class ResetGuiView : SHGUIview
    {
        public enum Action
        {
            ResetSecrets,
            ResetStory,
            ResetAll
        }

        private const int posX = 32;
        private const int posY = 12;
        private const int timerLimit = 200;

        private Action action;

        private SHGUIchoice guiChoice = null;
        private SHGUIview guiRestarting = null;

        private int timer = -1;

        public ResetGuiView(Action act)
        {
            dontDrawViewsBelow = false;
            action = act;

            switch(act)
            {
                case Action.ResetSecrets :
                    guiChoice = new SHGUIchoice("Do you really want to reset all secrets ?", posX, posY);

                    guiChoice.SetOnYes(ResetSecrets);
                    guiChoice.SetOnNo(Kill);

                    break;
                case Action.ResetStory:
                    guiChoice = new SHGUIchoice("Do you really want to reset the story (and secrets) ?", posX, posY);

                    guiChoice.SetOnYes(ResetStory);
                    guiChoice.SetOnNo(Kill);

                    break;
                case Action.ResetAll :
                    guiChoice = new SHGUIchoice("Do you really want to fully delete your save ?", posX, posY);

                    guiChoice.SetOnYes(ResetAll);
                    guiChoice.SetOnNo(Kill);
               
                    break;
            }

            AddSubView(guiChoice);
        }

        public override void Update()
        {
            base.Update();

            if (timer >= 0 && timer < timerLimit)
                timer++;
            else if(timer >= timerLimit)
            {
                timer = -1;

                Kill();

                switch (action)
                {
                    case Action.ResetSecrets:
                        SceneManager.LoadSceneAsync("PCFirst");
                        break;
                    case Action.ResetStory:
                        piOsMenuLauncher.ForceFirstLaunch = true;
                        APPlaunchlevel view = new APPlaunchlevel(LevelSetup.GetLevelInfoBySceneFileName("PCFirst", 0), "INTERnone");
                        SHGUI.current.AddViewOnTop(view);
                        break;
                    case Action.ResetAll:
                        SHGUI.current.AddViewToQueue(new SHGUItempview(2f));
                        SHGUI.current.gameObject.AddComponent<FirstLaunch>();
                        SHGUI.current.AddViewOnTop(new APPkill());
                        break;
                }
            }
        }

        public override void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
        {
            base.ReactToInputMouse(x, y, clicked, scroll);

            int yesOffset = 5;

            if(guiChoice != null && guiChoice.fadingOut == false && guiChoice.remove == false)
            {
                if (y == posY + yesOffset || y == posY + yesOffset + 1)
                {
                    if(TrySetGuiChoiceHighlight(y - posY - yesOffset))
                        SHGUI.current.PlaySound(SHGUIsound.tick);

                    if (clicked)
                        guiChoice.ReactToInputKeyboard(SHGUIinput.enter);
                }
            }
        }

        private void CreateRestartingView()
        {
            string display = "";

            switch(action)
            {
                case Action.ResetSecrets:
                    display = "Secrets Resetted !";
                    break;
                case Action.ResetStory:
                    display = "Story Resetted !";
                    break;
                case Action.ResetAll:
                    display = "Save DESTROYED !";
                    break;
            }

            int sizeX = 30;
            int sizeY = 6;

            guiRestarting = AddSubView(new SHGUIview());

            guiRestarting.x = SHGUI.current.resolutionX / 2 - sizeX / 2;
            guiRestarting.y = SHGUI.current.resolutionY / 2 - sizeY;

            guiRestarting.AddSubView(new SHGUIrect(0, 0, sizeX, sizeY, '0', ' ', 2));
            guiRestarting.AddSubView(new SHGUIframe(0, 0, sizeX, sizeY, 'r', null));

            guiRestarting.AddSubView(new SHGUItext(display, (sizeX - display.Length) / 2, 2, 'r', false));
            guiRestarting.AddSubView(new SHGUItext("Restarting piOs ...", (sizeX - 19) / 2, 4, 'r', false));

            guiRestarting.AddSubView(new SHGUILoadingIndicator(sizeX - 3, 4, 0.05f, 'r'));

            timer = 0;
        }

        public void ResetSecrets()
        {
            foreach (LevelInfo current in LevelSetup.Levels)
            {
                for (int i = 0; i < current.Secrets; i++)
                {
                    SaveManager.Instance.SaveData.Remove(current.SceneFileName + (i + 1) + "unlocked");
                }
                SaveManager.Instance.Save();
            }

            guiChoice.Kill();
            CreateRestartingView();
        }

        public void ResetStory()
        {
            //Reset secrets
            foreach (LevelInfo current in LevelSetup.Levels)
            {
                for (int i = 0; i < current.Secrets; i++)
                {
                    SaveManager.Instance.SaveData.Remove(current.SceneFileName + (i + 1) + "unlocked");
                }
                SaveManager.Instance.Save();
            }

            //Reset Story
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.WeaponAchievement.ClearCurrentGameStats();
            }

            guiChoice.Kill();
            CreateRestartingView();
        }

        public void ResetAll()
        {
            SaveManager.Instance.SaveData.Clear();
            SaveManager.Instance.Save();

            guiChoice.Kill();
            CreateRestartingView();
        }

        private bool TrySetGuiChoiceHighlight(int highlight)
        {
            if(guiChoice == null)
            {
                Debug.Log("SetGuiChoiceHighlight : guiChoice == null");
                return false;
            }

            FieldInfo field = guiChoice.GetType().GetField("highlightPosition", BindingFlags.NonPublic | BindingFlags.Instance);

            if(field == null)
            {
                Debug.Log("SetGuiChoiceHighlight : field == null");
                return false;
            }

            int currentVal = (int)field.GetValue(guiChoice);

            if (currentVal != highlight)
            {
                field.SetValue(guiChoice, highlight);
                return true;
            }

            return false;
        }
    }
}
