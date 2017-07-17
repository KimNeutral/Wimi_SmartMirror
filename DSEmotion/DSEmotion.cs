
namespace DSEmotion
{
    public enum Emotion
    {
        anger,
        contempt,
        disgust,
        fear,
        happiness,
        neutral,
        sadness,
        surprise,
        Err
    }

    public class EmotionUtil
    {
        public static Emotion GetEmotionByString(string emotion)
        {
            emotion = emotion.ToLower();
            switch (emotion)
            {
                case "anger":
                    return Emotion.anger;
                case "contempt":
                    return Emotion.contempt;
                case "disgust":
                    return Emotion.disgust;
                case "fear":
                    return Emotion.fear;
                case "happiness":
                    return Emotion.happiness;
                case "neutral":
                    return Emotion.neutral;
                case "sadness":
                    return Emotion.sadness;
                case "surprise":
                    return Emotion.surprise;
            }
            return Emotion.Err;
        }
    }
}
