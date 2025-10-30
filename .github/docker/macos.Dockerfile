FROM crazymax/osxcross:latest-ubuntu AS builder
ARG TARGET_TRIPLE

# Install additional dependencies
RUN apt-get update && apt-get install -y \
    cmake \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Download and build OpenSSL for macOS
WORKDIR /build
RUN wget https://www.openssl.org/source/openssl-3.0.15.tar.gz && \
    tar xzf openssl-3.0.15.tar.gz && \
    cd openssl-3.0.15 && \
    CC=${TARGET_TRIPLE}-clang \
    ./Configure darwin64-${TARGET_TRIPLE##*-}-cc --prefix=/opt/osxcross/SDK/MacOSX14.5.sdk/usr && \
    make -j$(nproc) && \
    make install_sw

# Build libssh2
COPY Native/libssh2 /src/libssh2
WORKDIR /src/libssh2/build

RUN cmake .. \
    -DCMAKE_SYSTEM_NAME=Darwin \
    -DCMAKE_C_COMPILER=${TARGET_TRIPLE}-clang \
    -DCMAKE_CXX_COMPILER=${TARGET_TRIPLE}-clang++ \
    -DCMAKE_BUILD_TYPE=Release \
    -DBUILD_SHARED_LIBS=ON \
    -DENABLE_ZLIB_COMPRESSION=ON \
    -DCRYPTO_BACKEND=OpenSSL \
    -DCMAKE_FIND_ROOT_PATH=/opt/osxcross/SDK/MacOSX14.5.sdk \
    -DOPENSSL_ROOT_DIR=/opt/osxcross/SDK/MacOSX14.5.sdk/usr \
    -DBUILD_EXAMPLES=OFF \
    -DBUILD_TESTING=OFF && \
    cmake --build . --config Release -j$(nproc) && \
    mkdir -p /output && \
    cp src/libssh2*.dylib /output/libssh2.dylib

FROM scratch
COPY --from=builder /output/* /
