TODO
====

AST
---
 - [ ] Reorder arguments for Node constructors to include optional start and end positions as first arguments for all.

Parser
------
 - [ ] Refactor ExpressionParser.
 - [ ] BUG: Line continuation with \ adds an extra word to commands.
 - [ ] special variables that start with $, cant be defined or assigned to, can be created by command such as try catch for error information $$errtype $$errmessage $$errdata.
  - [ ] hidden variables that start with $$, cant be defined or assigned to or read, but can be used by macros.

Commands
--------
 - [ ] Add position to errors thrown by argument parsers

Library
-------
### math
 - [ ] Add lerp function

### core
 - [x] Add repeat loop