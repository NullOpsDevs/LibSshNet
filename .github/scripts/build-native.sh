#!/bin/bash
set -e

PLATFORM=$1
TARGET_TRIPLE=$2
DOCKERFILE_PLATFORM=$3

mkdir -p output

if [[ "$PLATFORM" == win-* ]]; then
  # Windows build using MinGW
  docker buildx build \
    --platform "$DOCKERFILE_PLATFORM" \
    --build-arg TARGET_TRIPLE="$TARGET_TRIPLE" \
    --output type=local,dest=./output \
    -f .github/docker/windows.Dockerfile .

elif [[ "$PLATFORM" == osx-* ]]; then
  # macOS build using pre-built osxcross image
  docker buildx build \
    --platform "$DOCKERFILE_PLATFORM" \
    --build-arg TARGET_TRIPLE="$TARGET_TRIPLE" \
    --output type=local,dest=./output \
    -f .github/docker/macos.Dockerfile .

else
  # Linux build
  docker buildx build \
    --platform "$DOCKERFILE_PLATFORM" \
    --output type=local,dest=./output \
    -f .github/docker/linux.Dockerfile .
fi

# Create runtime directory structure
mkdir -p "NullOpsDevs.LibSsh/runtimes/$PLATFORM/native"

# Copy library to correct location
if [[ "$PLATFORM" == win-* ]]; then
  cp output/libssh2.dll "NullOpsDevs.LibSsh/runtimes/$PLATFORM/native/"
elif [[ "$PLATFORM" == osx-* ]]; then
  cp output/libssh2.dylib "NullOpsDevs.LibSsh/runtimes/$PLATFORM/native/"
else
  # Copy and create symlink without version suffix
  cp output/libssh2.so* "NullOpsDevs.LibSsh/runtimes/$PLATFORM/native/"
  # Find the actual .so file and create a symlink
  cd "NullOpsDevs.LibSsh/runtimes/$PLATFORM/native/"
  if [ -f libssh2.so.1 ]; then
    ln -sf libssh2.so.1 libssh2.so
  fi
fi
