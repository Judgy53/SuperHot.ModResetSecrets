using UnityEngine;

namespace Mod
{
    public class ModResetSecrets : MonoBehaviour
    {
        private static bool DEBUG_GUI = false;

        private SHGUIcommanderbutton separatorButton = null;
        private SHGUIcommanderbutton resetSecretsButton = null;
        private SHGUIcommanderbutton resetStoryButton = null;
        private SHGUIcommanderbutton resetAllButton = null;

        public void Update()
        {
            if(SHGUI.current != null) // is there a GUI displayed
            {
                SHGUIview guiView = SHGUI.current.GetInteractableView();

                if(guiView != null) // is the current view the main view
                {
                    if (guiView is SHGUIcommanderview)
                    {
                        SHGUIcommanderview guiCommander = guiView as SHGUIcommanderview;

                        if (guiCommander.path.EndsWith("SETTINGS\\") && guiCommander.buttons.Count == 6) //is in the settings menu and we didn't add any buttons yet
                        {
                            separatorButton = new SHGUIcommanderbutton("------------│--------", 'z', null).SetListLink(guiCommander).SetData(string.Empty).SetLocked(true);
                            resetSecretsButton = new SHGUIcommanderbutton("delsecr.bat │--FILE->", 'w', delegate (SHGUIcommanderbutton x) { CreateChoiceView(ResetGuiView.Action.ResetSecrets); }).SetListLink(guiCommander).SetData("* reset secrets found \n* leaves story progression\n* leaves records intact");
                            resetStoryButton = new SHGUIcommanderbutton("newsave.bat │--FILE->", 'w', delegate (SHGUIcommanderbutton x) { CreateChoiceView(ResetGuiView.Action.ResetStory); }).SetListLink(guiCommander).SetData("* reset secrets found \n* reset story progression\n* leaves records intact");
                            resetAllButton = new SHGUIcommanderbutton("format.bat  │--FILE->", 'w', delegate (SHGUIcommanderbutton x) { CreateChoiceView(ResetGuiView.Action.ResetAll); }).SetListLink(guiCommander).SetData("DESTROY YOUR SAVE, YOUR MIND AND YOUR BANK ACCOUNT !!!!11!!1!! \n\n\n\n\n\n\n\n\njk");

                            guiCommander.AddButtonView(separatorButton);
                            guiCommander.AddButtonView(resetSecretsButton);
                            guiCommander.AddButtonView(resetStoryButton);
                            guiCommander.AddButtonView(resetAllButton);
                        }
                    }
                }
            }
        }

        private void CreateChoiceView(ResetGuiView.Action action)
        {
            SHGUI.current.AddViewOnTop(new ResetGuiView(action));
        }

        public void OnGUI()
        {
            if (DEBUG_GUI == false)
                return;

            int basePosX = 900;

            SHGUI gui = SHGUI.current;

            if (gui == null)
                GUI.Label(new Rect(basePosX, 0, 100, 25), "No gui");
            else
            {
                GUI.Label(new Rect(basePosX, 0, 400, 25), gui.GetType().Name + " (" + gui.name + ")");

                SHGUIview guiInter = gui.GetInteractableView();

                if (guiInter == null)
                    GUI.Label(new Rect(basePosX, 25, 100, 25), "No view");
                else
                {
                    GUI.Label(new Rect(basePosX, 25, 400, 25), guiInter.GetType().Name);

                    if (guiInter is SHGUIcommanderview)
                    {
                        SHGUIcommanderview guiComm = (SHGUIcommanderview)guiInter;

                        int i = 0;
                        foreach (SHGUIcommanderbutton but in guiComm.buttons)
                        {
                            GUI.Label(new Rect(basePosX, 50 + i++ * 25, 400, 25), but.ButtonText + " (" + but.IsLocked + ")");
                        }
                    }
                }
            }
        }
    }
}
