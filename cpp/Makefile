.PHONY: all protobuf gen test clean

all: test

protobuf:
	make -C .. protobuf-cpp

gen: protobuf
	PYTHONPATH=../python ./krpci_generator.py
	mkdir -p include
	mv krpci.hpp include/
	mv krpci_generated.cpp src/

test: gen
	g++ -o krpci_test \
		src/krpci_test.cpp \
		src/krpci_base.cpp \
		src/krpci_generated.cpp \
		src/kRPC/Schema/KRPC.pb.cc \
		-Iinclude -I./ -Isrc/kRPC/Schema/ \
		-lboost_system -lboost_thread `pkg-config --libs --cflags protobuf`

clean:
	rm -rf include src/krpci_generated.cpp src/kRPC templates/*.py templates/*.py templates/*.bak.py
	find . -name "*.pyc" -exec rm -rf {} \;

