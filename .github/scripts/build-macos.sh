#!/bin/bash
set -e

PLATFORM=$1
ARCH=$2

# Install dependencies
brew install cmake openssl@3

# Build libssh2
cd Native/libssh2
mkdir -p build
cd build

cmake .. \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DENABLE_ZLIB_COMPRESSION=ON \
  -DCRYPTO_BACKEND=OpenSSL \
  -DOPENSSL_ROOT_DIR=$(brew --prefix openssl@3) \
  -DCMAKE_OSX_ARCHITECTURES=$ARCH \
  -DBUILD_EXAMPLES=OFF \
  -DBUILD_TESTING=OFF

cmake --build . --config Release -j$(sysctl -n hw.ncpu)

# Create output directory
mkdir -p "../../../NullOpsDevs.LibSsh/runtimes/$PLATFORM/native"

# Copy the dylib
cp src/libssh2*.dylib "../../../NullOpsDevs.LibSsh/runtimes/$PLATFORM/native/libssh2.dylib"
