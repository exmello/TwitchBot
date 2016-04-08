using TwitchBot.Data.Dictionary;

namespace TwitchBot.Data
{
    public interface IDictionaryRepository
    {
        WordsResult GetRandomWords();
        DefinitionResult GetDefinition(string word);
    }
}
