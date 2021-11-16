bE	: bE '|' '|' bT 
	| bT; 
bT	: bT '&' '&' bF 
	| bF; 
bF	: '!' bF 
	| ' ('bE')' 
	| '1' 
	| '0';