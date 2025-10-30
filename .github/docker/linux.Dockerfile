FROM debian:bookworm-slim

RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    libssl-dev \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

COPY Native/libssh2 /src/libssh2
WORKDIR /src/libssh2/build

RUN cmake .. \
    -DCMAKE_BUILD_TYPE=Release \
    -DBUILD_SHARED_LIBS=ON \
    -DENABLE_ZLIB_COMPRESSION=ON \
    -DCRYPTO_BACKEND=OpenSSL \
    -DBUILD_EXAMPLES=OFF \
    -DBUILD_TESTING=OFF && \
    cmake --build . --config Release -j$(nproc) && \
    mkdir -p /output && \
    cp src/libssh2.so* /output/

FROM scratch
COPY --from=0 /output/* /
