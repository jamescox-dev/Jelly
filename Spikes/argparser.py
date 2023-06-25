import json


class ParseResult:
    def __init__(self, arg_values, processed_args=0, status='success'):
        self.arg_values = arg_values
        self.processed_args = processed_args
        self.status = status

    def __repr__(self):
        return f'''ParseResult(status={repr(self.status)}, processed_args={self.processed_args}, arg_values={json.dumps(self.arg_values, indent=2)})'''


class Single:
    def __init__(self, name):
        self.name = name
    
    def parse(self, pos, args):
        if pos < len(args):
            return ParseResult({self.name: args[pos]}, pos + 1)
        
        return ParseResult({'name': [self.name]}, pos, 'missing')


class Keyword:
    def __init__(self, keyword):
        self.keyword = keyword
    
    def parse(self, pos, args):
        if pos < len(args) and args[pos] == self.keyword:
            return ParseResult({}, pos + 1)
        
        return ParseResult({'name': [repr(self.keyword)]}, pos, 'missing')


class Seq:
    def __init__(self, *parsers):
        self.parsers = parsers
    

    def parse(self, pos, args):
        arg_values = {}

        for parser in self.parsers:
            result = parser.parse(pos, args)
            if result.status == 'success':
                pos = result.processed_args
                arg_values |= result.arg_values
            else:
                return result

        return ParseResult(arg_values, pos)


class NoMoreThan:
    def __init__(self, parser):
        self.parser = parser

    def parse(self, pos, args):
        result = self.parser.parse(pos, args)
        if result.status == 'success' and result.processed_args < len(args):
            return ParseResult({}, result.processed_args, 'unexpected')
        
        return result


class Either:
    def __init__(self, *parsers):
        self.parsers = parsers

    def parse(self, pos, args):
        best_match_failure = ParseResult({}, 0, 'unexpected')
        for parser in self.parsers:
            result = parser.parse(pos, args)
            if result.status == 'success':
                return result
            elif result.status == 'missing' and result.processed_args == best_match_failure.processed_args:
                best_match_failure.arg_values = { 'name': list(set(best_match_failure.arg_values['name']) | set(result.arg_values['name'])) }
            elif result.processed_args > best_match_failure.processed_args:
                best_match_failure = result

        return best_match_failure


class OneOrMore:
    def __init__(self, parser):
        self.parser = parser

    def parse(self, pos, args):
        matched = False
        arg_values = {}
        while True:
            result = self.parser.parse(pos, args)
            if result.status == 'success':
                pos = result.processed_args
                for key, value in result.arg_values.items():
                    arg_values.setdefault(key, []).append(value)
                matched = True
            else:
                if not matched:
                    return result
                break

        return ParseResult(arg_values, pos)


class ParserTest:
    def __init__(self, name, parser, args_list):
        self.name = name
        self.parser = parser
        self.args_list = args_list
    
    def run(self):
        for args in self.args_list:
            print(self.name, *args)
            print(self.parser.parse(0, args))


def main():
    tests = [
        ParserTest(
            'for',
            Either(
                NoMoreThan(Seq(Single('key'), Single('value'), Keyword('of'), Single('dict'), Single('body'))),
                NoMoreThan(Seq(Single('value'), Keyword('of'), Single('dict'), Single('body'))),
                NoMoreThan(Seq(Single('index'), Single('value'), Keyword('in'), Single('list'), Single('body'))),
                NoMoreThan(Seq(Single('value'), Keyword('in'), Single('list'), Single('body'))),
                NoMoreThan(Seq(Single('it'), Keyword('='), Single('start'), Keyword('to'), Single('end'), Keyword('step'), Single('step'), Single('body'))),
                NoMoreThan(Seq(Single('it'), Keyword('='), Single('start'), Keyword('to'), Single('end'), Single('body'))),
            ),
            [
                ['name', 'age', 'of', 'ages_by_name', '{}'],
                ['age', 'of', 'ages_by_name', '{}'],
                ['postion', 'score', 'of', 'hiscores', '{}'],
                ['score', 'of', 'hiscores', '{}'],
                ['a', '=', '1', 'to', '10', 'step', '2', '{}'],
                ['a', '=', '1', 'to', '10', '{}'],
            ]    
        ),
        ParserTest(
            'if',
            Either(
                NoMoreThan(Seq(Single('condition'), Single('body'), OneOrMore(Seq(Keyword('elif'), Single('elifcond'), Single('elifbody'))), Keyword('else'), Single('else'))),
                NoMoreThan(Seq(Single('condition'), Single('body'), OneOrMore(Seq(Keyword('elif'), Single('elifcond'), Single('elifbody'))))),
                NoMoreThan(Seq(Single('condition'), Single('body'), Keyword('else'), Single('else'))),
                NoMoreThan(Seq(Single('condition'), Single('body'))),
            ),
            [ 
                ['(???)', '{}'],
                ['(???)', '{this}', 'else', '{that}'],
                ['(when)', '{this}', 'elif', '(somethingelse)', '{that}', 'elif', '(someotherthing)', '{theother}'],
                ['(when)', '{this}', 'elif', '(somethingelse)', '{that}', 'else', '{theother}'],
            ]
        ),
        ParserTest(
            'var',
            Either(
                NoMoreThan(Seq(Single('varname'), Keyword('='), Single('value'))),
                NoMoreThan(Seq(Single('varname'), Keyword('='))),
                NoMoreThan(Single('varname')),
            ),
            [
                ['$a'],
                ['$a', '='],
                ['$a', '=', 'value'],
            ]
        ),
        ParserTest(
            'try',
            Either(
                NoMoreThan(Seq(Single('body'), Keyword('finally'), Single('finally'))),
                NoMoreThan(Seq(Single('body'), OneOrMore(Seq(Keyword('except'), Single('exceptpattern'), Single('exceptbody'))), Keyword('finally'), Single('finally'))),
                NoMoreThan(Seq(Single('body'), OneOrMore(Seq(Keyword('except'), Single('exceptpattern'), Single('exceptbody'))))),
            ),
            [
                ['{try this}', 'finally', '{do this}'],
                ['{try this}', 'except', '/error', '{do this on error}'],
                ['{try this}', 'except', '/error', '{do this on error}', 'finally', '{finish}'],
            ]
        ),
    ]
    
    for test in tests:
        test.run()


if __name__ == '__main__': main()