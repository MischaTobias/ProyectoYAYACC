Expr    :   Expr '+' Term
        |   Expr '-' Term
        |   Term;

Term    :   'x'
        |   '(' Expr ')';
