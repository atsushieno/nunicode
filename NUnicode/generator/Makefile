
gen = generate-east-asian-width
genexe = $(gen).exe
gencs = $(gen).cs

all: EastAsianWidth.dat EastAsianWidth.opt

EastAsianWidth.dat EastAsianWidth.opt: $(genexe)
	mono --debug $(genexe)

$(genexe): $(gencs)
	mcs -debug $(gencs)

clean:
	rm -rf EastAsianWidth.dat EastAsianWidth.opt

