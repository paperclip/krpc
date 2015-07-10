.PHONY: all protobuf generate build install clean

all: build

protobuf:
	make -C .. protobuf-cpp

generate: protobuf
	PYTHONPATH=../python ./generate

build: generate
	./waf configure build

install:
	./waf install

clean:
	-./waf clean
	rm -rf include/krpci.hpp include/KRPC.pb.h src/krpci.cpp src/KRPC.pb.cc
	find . -name "*.pyc" -exec rm -rf {} \;
	rm -rf build
