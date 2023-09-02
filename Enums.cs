using static DataPearl.AbstractDataPearl;
using static Menu.MenuScene;
using static Menu.SlideShow;

namespace TheAmbidextrous;

//ported from Pearlcat mod
//link: https://github.com/forthbridge/pearlcat/blob/a9b8f9f7632bb1645408a1f89b003565777343ea/src/Enums.cs
public static class Enums
{
    public static SlugcatStats.Name Ambidextrous = new(nameof(Ambidextrous), false);

    /*
    public static class SSOracle
    {
        public static Conversation.ID Pearlcat_SSConvoFirstMeet = new(nameof(Pearlcat_SSConvoFirstMeet), true);
        public static Conversation.ID Pearlcat_SSConvoFirstLeave = new(nameof(Pearlcat_SSConvoFirstLeave), true);

        public static Conversation.ID Pearlcat_SSConvoRMPearlInspect = new(nameof(Pearlcat_SSConvoRMPearlInspect), true);
        public static Conversation.ID Pearlcat_SSConvoTakeRMPearl = new(nameof(Pearlcat_SSConvoTakeRMPearl), true);
        public static Conversation.ID Pearlcat_SSConvoSickPup = new(nameof(Pearlcat_SSConvoSickPup), true);

        public static Conversation.ID Pearlcat_SSConvoSSPearl = new(nameof(Pearlcat_SSConvoSSPearl), true);
        public static Conversation.ID Pearlcat_SSConvoASPearlBlue = new(nameof(Pearlcat_SSConvoASPearlBlue), true);
        public static Conversation.ID Pearlcat_SSConvoASPearlRed = new(nameof(Pearlcat_SSConvoASPearlRed), true);
        public static Conversation.ID Pearlcat_SSConvoASPearlGreen = new(nameof(Pearlcat_SSConvoASPearlGreen), true);
        public static Conversation.ID Pearlcat_SSConvoASPearlBlack = new(nameof(Pearlcat_SSConvoASPearlBlack), true);
        public static Conversation.ID Pearlcat_SSConvoASPearlYellow = new(nameof(Pearlcat_SSConvoASPearlYellow), true);

        public static SSOracleBehavior.Action Pearlcat_SSActionGeneral = new(nameof(Pearlcat_SSActionGeneral), true);
        public static SSOracleBehavior.SubBehavior.SubBehavID Pearlcat_SSSubBehavGeneral = new(nameof(Pearlcat_SSSubBehavGeneral), true);

        public static SlugcatStats.Name PearlcatPebbles = new(nameof(PearlcatPebbles), true);
    }
    */

    public static class Pearls
    {
        public static readonly DataPearlType NC_AaTEoaU = new(nameof(NC_AaTEoaU), false);
        public static readonly DataPearlType NC_AmbiStoryPearl = new(nameof(NC_AmbiStoryPearl), false);
        //public static DataPearlType RM_Pearlcat = new(nameof(RM_Pearlcat), false);
        //public static DataPearlType SS_Pearlcat = new(nameof(SS_Pearlcat), false);

        //public static DataPearlType AS_PearlBlue = new(nameof(AS_PearlBlue), false);
        //public static DataPearlType AS_PearlYellow = new(nameof(AS_PearlYellow), false);
        //public static DataPearlType AS_PearlGreen = new(nameof(AS_PearlGreen), false);
        //public static DataPearlType AS_PearlRed = new(nameof(AS_PearlRed), false);
        //public static DataPearlType AS_PearlBlack = new(nameof(AS_PearlBlack), false);
    }

    /* contents of TheAmbidextrous-master/mod/modify/custompearls.txt
    [ADD]NC_AaTEoaU : d5e8ff : 4f6781 : AaTEoaU_conversion
    [ADD]NC_AmbiStoryPearl : bfa6a6 : d7ff63 : AmbiStoryPearl_conversion
    */

    /*
    public static class Scenes
    {
        public static SceneID Slugcat_Pearlcat = new(nameof(Slugcat_Pearlcat), false);
        public static SceneID Slugcat_Pearlcat_Sick = new(nameof(Slugcat_Pearlcat_Sick), false);
        public static SceneID Slugcat_Pearlcat_Ascended = new(nameof(Slugcat_Pearlcat_Ascended), false);

        public static SceneID Slugcat_Pearlcat_Sleep = new(nameof(Slugcat_Pearlcat_Sleep), false);

        public static SceneID Slugcat_Pearlcat_Statistics_Ascended = new(nameof(Slugcat_Pearlcat_Statistics_Ascended), false);
        public static SceneID Slugcat_Pearlcat_Statistics_Sick = new(nameof(Slugcat_Pearlcat_Statistics_Sick), false);


        public static SlideShowID Pearlcat_AltOutro = new(nameof(Pearlcat_AltOutro), false);
    }
    */
}