# Detect_Text_In_Game
========================
A lot of duplicate text in a lot of files, this tool will make duplicate text becomes different. So it's easy to detect what real text used in game.
Then use 010 editor to find real text in file.

e.g:
	1.dat, 2.dat, 3.dat has duplicate "abcdef" text 
	After use this tool:
	"ab0000" (1.dat)
	"ab0001" (2.dat)
	"ab0002" (3.dat)

Currently support:
========================
- Encoding: unicode big endian.
- File size < 150 MB
- Number of files < 1000

TODO
========================
- [] Encode utf8, unicode little endian.
- [] File size > 150 MB
- [] Number of files > 1000