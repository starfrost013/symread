# SYMREAD: A tool to read early (mid-80s) Microsoft SYMDEB format .SYM files. They were used in early OS/2 1.0 builds, Windows 1.x, and Windows 2.x

**NOTE: This tool has not been tested with and probably will not work with Windows 3.x and 95 format .SYM files. They seem to be different at a cursory inspection. **

# File format documentation:

## Header (16 bytes):

0x00 -			WORD -		Total number of symbols (or entries)


0x02 -			WORD -		Always 0 (Reserved)


0x04 -			WORD -		Unknown


0x06 -			WORD -		Number of entries in constant section


0x08 -			WORD -		Offset to start of unknown data for first seg


0x0A -			WORD -		Number of segments	


0x0C -			WORD -		Unknown (0x02, 0x06, 0x07...number of code segments?)


0x0E -			BYTE -		Unknown


0x0F -			BYTE -		Length of string of binary. Use readstring in c#


0x10 -			STRING -	Name of binary 


Then absolute-addressed constants (?) using the symbol format, and then each segment is defined:


0x00 -			BYTE -		Unknown.


The symbols are then addressed in the same format as in actual code segments (all addresses are from the start of the code).


## Segment header (21 + length of function name bytes):


0x00 -			WORD -		Unknown. :(


0x02 -			WORD -		Number of symbols in this segment


0x04 -			WORD -		Size of symbol data (unknown data is [offset] + [this value])


0x06 -			WORD -		Segment number


0x08-0x0F -		QWORD -		Reserved. (seemingly all zeros everywhere)


0x10 -			WORD -		Unknown, but always zero


0x12 -			WORD -		Unknown, but always 0xFF00 (65280)...maximum you can load it to?


0x14 -			BYTE -		Length of segment name string.


0x15 -			STRING -	Segment name


After the segment header, you have the symbols themselves:


## Symbol format


0x00 -			WORD -		Offset from segment start that the symbol resides


0x02 -			BYTE -		Length of string


0x03..0x03+length -	WORD -		Name of symbol


For the constants, every segment you have some unknown data after the symbols themselves - this data increments for every single symbol, and is duplicated for every single symbol, before being padded with 00 to the nearest sixteen bytes.
Size of function? "Line number" data? But turning off line numbers makes there be no difference...
