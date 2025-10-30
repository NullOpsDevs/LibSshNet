#!/bin/bash
set -e

PLATFORM=$1
ARCH=$2

# Install dependencies
brew install cmake openssl@3

# Determine the correct OpenSSL path based on architecture
if [[ "$ARCH" == "x86_64" ]]; then
  # For x86_64 builds on ARM64 runners, we need to build OpenSSL from source
  OPENSSL_PREFIX="/tmp/openssl-$ARCH"

  if [ ! -d "$OPENSSL_PREFIX" ]; then
    mkdir -p /tmp/openssl-build
    cd /tmp/openssl-build

    # Download and build OpenSSL for x86_64
    curl -L https://www.openssl.org/source/openssl-3.0.15.tar.gz | tar xz
    cd openssl-3.0.15

    ./Configure darwin64-x86_64-cc --prefix="$OPENSSL_PREFIX"
    make -j$(sysctl -n hw.ncpu)
    make install_sw
  fi
else
  # For ARM64, use Homebrew's OpenSSL
  OPENSSL_PREFIX=$(brew --prefix openssl@3)
fi

# Build libssh2
cd "$GITHUB_WORKSPACE/Native/libssh2"
rm -rf build
mkdir -p build
cd build

cmake .. \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DENABLE_ZLIB_COMPRESSION=ON \
  -DCRYPTO_BACKEND=OpenSSL \
  -DOPENSSL_ROOT_DIR="$OPENSSL_PREFIX" \
  -DCMAKE_OSX_ARCHITECTURES=$ARCH \
  -DBUILD_EXAMPLES=OFF \
  -DBUILD_TESTING=OFF

cmake --build . --config Release -j$(sysctl -n hw.ncpu)

# Create output directory using absolute path
OUTPUT_DIR="$GITHUB_WORKSPACE/NullOpsDevs.LibSsh/runtimes/$PLATFORM/native"
mkdir -p "$OUTPUT_DIR"

# Copy the dylib (find the actual versioned file, not the symlinks)
DYLIB_FILE=$(find src -name "libssh2.*.*.*.dylib" -type f | head -n 1)
if [ -n "$DYLIB_FILE" ]; then
  cp "$DYLIB_FILE" "$OUTPUT_DIR/libssh2.dylib"
else
  echo "Error: Could not find libssh2 dylib file"
  exit 1
fi
