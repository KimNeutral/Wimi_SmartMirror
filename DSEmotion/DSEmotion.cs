
namespace DSEmotion
{
    public enum Emotion
    {
        none,
        anger,
        contempt,
        disgust,
        fear,
        happiness,
        neutral,
        sadness,
        surprise,
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
            return Emotion.none;
        }

        public static string GetCommentByEmotion(Emotion emotion)
        {
            switch (emotion)
            {
                case Emotion.anger:
                    return "화가 나셨군요.";
                case Emotion.contempt:
                    return "멸시하시고 있으시군요.";
                case Emotion.disgust:
                    return "역겨우신가요?";
                case Emotion.fear:
                    return "공포스러우신가요?";
                case Emotion.happiness:
                    return "행복해보여요.";
                case Emotion.neutral:
                    return "평온해보이네요.";
                case Emotion.sadness:
                    return "슬픈일 있으신가요?";
                case Emotion.surprise:
                    return "놀라보여요.";
            }
            return null;
        }
    }
}
