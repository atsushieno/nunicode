
all: build

build:
	xbuild

clean:
	xbuild /t:Clean

test: 
	cd NUnicode.Tests && make test

