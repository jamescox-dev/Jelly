namespace Jelly.Shell;

public class ShellConfig
{
    public string WelcomeMessage => @"      _     _ _
     | `___| ` `_   _
 _   | / _ ` | | ` | `
| `__| | __/ | | |_| | 
 `____/`___`__`_`__, | {0}
                `___/";

    public string Prompt => "> ";
    
    public string ContinuationPrompt => ". ";
}