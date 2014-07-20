
OUTDIR = bin/Debug
REF = bin/Debug/NUnicode.dll
DLL = $(OUTDIR)/NUnicode.Tests.dll
SRC = */*.cs

all: $(DLL)

$(DLL): $(REF) $(SRC)
	mcs -debug -out:$(DLL) -t:library $(SRC) -pkg:nunit -r:$(REF)

$(REF): $(OUTDIR)
	cd ../NUnicode && make
	cp ../NUnicode/bin/Debug/NUnicode.dll* $(OUTDIR)

$(OUTDIR):
	mkdir -p $(OUTDIR)

test:
	$(NUNIT_CONSOLE) $(DLL)

