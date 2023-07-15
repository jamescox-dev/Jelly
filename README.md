Jelly🍒
=======
A beginner friendly scripting language, with simple (or quirky syntax).

`jello, world`
--------------
Jelly programs (or *scripts*) are made up of a series of *commands*, each command is separated by either a new line or semicolon.  Commands are built out of words each separated by whitespace.  Optionally, *comments* can be added to describe the code to the programmer.  A comment starts with a hash-symbol and ends at the end of the line.

```
# An example program.
print jello, world
input what next!?
```

In the script above we have two commands.  The first word of a command is its *name*, and the following words are called it's *arguments*.  The first command in our script is the **`print`** command, it displays text to the user, by displaying each of its arguments separated by a single space.  The second command is the **`input`** command, like the **`print`** command it displays its arguments to the user, but then it waits for the user to enter some text.  If you run the script you will see the following output, the script then waits for your input.

```
jello, world
what next!?
```

### Variables
Jelly has *variables* -- names that can be associated with values.  You define a variable with the **`var`** command, and then reference the variable by its name proceeded with a dollar-sign.

```
var $answer = 42
print The answer to life, the universe, and everything: $answer
```

The **`var`** command takes 3 arguments, the first is the name of the variable to create, the second is the equals-sign (yes, this is an argument), and finally the value to associate with the variable.  Later when we use the `$answer` as an argument to the **`print`** command the variables value is substituted in its place.  Running the script will give us the following output:

```
The answer to life, the universe, and everything: 42
```

### Express Yourself!
Jelly has a special way to write mathematical *expressions*.  Expressions are written in parenthesis, and as with variables, when Jelly encounters an expression, it is evaluated and the result is used in it's place.  Expressions can consist of *operators*, *values*, *functions*, and *variables*.

Consider the variation on our previous example now we can actually calculate the answer to life, the universe, and everything.

```
var $life = 7
var $universe = 3
var $everything = 2
var $answer = ($life * $universe * $everything)
print The answer to life, the universe, and everything: $answer
```

### Command Substitution
Along with variables and expressions, the *return value* of a command can be substituted, this is done by wrapping the command and its arguments in braces.  In the example below we use the **`input`** command to read input from the user, and assign it to a variable, the variable is then used to display a friendly greeting to the user.

```
var $name = {input What is your name?}
print Hi! $name
```

### When You Need To Escape
As words are separated by whitespace, and other characters have spacial meanings it can be awkward if you what to include these *special characters* in a word you wish to pass to a command.  Jelly has a few ways to include (or *escape*) these special characters.

#### The Escape Character
If your proceed any character with a backslash, the backslash is ignored and the following character will be included in the word.

```
print \$name backslashes look like this: \\
```
Will print:
```
$name backslashes look like this: \
```
Ignoring the dollar-sign as marking a variable and just includes it as character in the word, note how the same rule applies to the backslash itself.

#### You Can Quote Me On That
Another way to include special characters is by surrounding them with matching quotes (single or double). Every character inside the quoted word will be include as-is, with the following exceptions, escape characters and command substitutions will still work as they normally do.  This is useful for formatting messages.

```
var $name = {input 'What is you name? '}
print "Hello, '{$name}'"
```
Will print:
```
What is your name? James
print Hello, 'James'
```

#### Nesting Quotes
Finally you can use brackets as *nesting quotes*, these quotes keep track of *opening* and *closing* quotes, and only once they are balanced does the quoted word end.  Everything between the opening and closing quote is included as-is, no substitutions (well nearly none, an opening or closing brace can be escaped with a backslash).  This format is often used for storing a list of values.

```
print [This [is] a test \[ so there!]
```
Will print:
```
This [is] a test \[ so there!
```
Notice how the nested brackets are included in the word, and how the backslash stopped the last opening bracket from being counted, but still included the backslash and the bracket.

[//]: # (TODO:  Introduction to the core library.)