.PHONY: all protobuf gen test clean

all: test

protobuf:
	make -C .. protobuf-cpp

generate: protobuf
	PYTHONPATH=../python ./generate

test: generate
	./waf configure build

clean:
	-./waf clean
	rm -rf include/krpci.hpp include/KRPC.pb.h src/krpci.cpp src/KRPC.pb.cc
	find . -name "*.pyc" -exec rm -rf {} \;
	rm -rf build
