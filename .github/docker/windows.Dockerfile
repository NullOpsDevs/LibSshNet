FROM debian:bookworm-slim
ARG TARGET_TRIPLE

RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    mingw-w64 \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Download and build OpenSSL for Windows
WORKDIR /build
RUN wget https://www.openssl.org/source/openssl-3.0.15.tar.gz && \
    tar xzf openssl-3.0.15.tar.gz && \
    cd openssl-3.0.15 && \
    ./Configure mingw64 --cross-compile-prefix=${TARGET_TRIPLE}- --prefix=/opt/openssl-${TARGET_TRIPLE} && \
    make -j$(nproc) && \
    make install_sw

# Build libssh2
COPY Native/libssh2 /src/libssh2
WORKDIR /src/libssh2/build

RUN cmake .. \
    -DCMAKE_SYSTEM_NAME=Windows \
    -DCMAKE_C_COMPILER=${TARGET_TRIPLE}-gcc \
    -DCMAKE_RC_COMPILER=${TARGET_TRIPLE}-windres \
    -DCMAKE_BUILD_TYPE=Release \
    -DBUILD_SHARED_LIBS=ON \
    -DENABLE_ZLIB_COMPRESSION=OFF \
    -DCRYPTO_BACKEND=OpenSSL \
    -DOPENSSL_ROOT_DIR=/opt/openssl-${TARGET_TRIPLE} \
    -DBUILD_EXAMPLES=OFF \
    -DBUILD_TESTING=OFF && \
    cmake --build . --config Release -j$(nproc) && \
    mkdir -p /output && \
    cp src/libssh2.dll /output/ || cp src/libssh2-*.dll /output/libssh2.dll

FROM scratch
COPY --from=0 /output/* /
