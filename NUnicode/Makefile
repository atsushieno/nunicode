
OUTDIR = bin/Debug
DLL = $(OUTDIR)/NUnicode.dll
RES = generator/EastAsianWidth.dat
OPT = generator/EastAsianWidth.opt

all: $(DLL)

$(DLL): $(RES) */*.cs
	mkdir -p $(OUTDIR)
	mcs -out:$(DLL) */*.cs -res:$(RES) -res:$(OPT)

$(RES) $(OPT):
	cd generator && make

clean:
	rm -rf bin/Debug/NUnicode.dll*
	cd generator && make clean

