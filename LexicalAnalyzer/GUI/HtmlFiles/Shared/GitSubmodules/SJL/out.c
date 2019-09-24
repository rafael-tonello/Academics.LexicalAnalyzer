Cannot open file ./

BIN2C V2.0 - Free binary file to C file converter

	Syntax:  BIN2C input_file_name [ > output_file_name ]

	by JRVV @ ELECSAN S.A. 2016, based on DEC.CPP by Dustin Caldwell.

	Reads a file and prints the C hexadecimal equivalent of each byte
	comma-separated. To write the output to a file, use the redirection
	operator like in this example:

	    BIN2C MyWebPage.html > MyWebPageArray.c

	This will create a standard C file called MyWebPageArray.c with the
	contents of MyWebPage.html as an unsigned char array.

