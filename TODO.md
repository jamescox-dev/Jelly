TODO
====

Parser
------
 - [x] Add position to errors thrown by the parsers.
 - [ ] Refactor ExpressionParser.
 - [ ] Fix bug where specifying a dictionary indexer but not following with an expression throws a null pointer exception.

Commands
--------
 - [ ] Combine `Variable/ValueGroupCommands` into one `ValueGroupCommand` using a new signature such as `list $name = add Vic Bob`.
 - [ ] Add position to errors thrown by argument parsers.

Values
------
 - [ ] Create ValueIndexers for getting and setting values in nested data-structures.