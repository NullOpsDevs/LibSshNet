# Performance and Reliability

## Stress Testing

A local stress test was conducted with 2,000 parallel SSH connections to a Docker container on an AMD Ryzen 9 5900X system, completing in 7,921ms.

### Test Environment

**Docker Compose Configuration** (`docker-compose.yml`):

```yaml
services:
  ssh-server:
    build:
      context: ./docker
      dockerfile: Dockerfile
    container_name: libssh-test-server
    hostname: ssh-test
    network_mode: "host"
    restart: unless-stopped
    ulimits:
      nofile:
        soft: 65536
        hard: 65536
      nproc:
        soft: 32768
        hard: 32768
    healthcheck:
      test: ["CMD", "nc", "-z", "localhost", "2222"]
      interval: 5s
      timeout: 3s
      retries: 10
```

**Dockerfile**:

```Docker
FROM alpine:latest

# Install OpenSSH server
RUN apk add --no-cache openssh-server openssh-keygen

# Create user
RUN adduser -D -s /bin/sh user && \
    echo "user:12345" | chpasswd

# Setup SSH directories
RUN mkdir -p /run/sshd /home/user/.ssh && \
    chmod 700 /home/user/.ssh && \
    chown -R user:user /home/user/.ssh

# Generate host keys
RUN ssh-keygen -A

# Create sshd_config with high limits
RUN echo 'Port 2222' > /etc/ssh/sshd_config && \
    echo 'PermitRootLogin no' >> /etc/ssh/sshd_config && \
    echo 'PasswordAuthentication yes' >> /etc/ssh/sshd_config && \
    echo 'PubkeyAuthentication yes' >> /etc/ssh/sshd_config && \
    echo 'AuthorizedKeysFile /home/user/.ssh/authorized_keys' >> /etc/ssh/sshd_config && \
    echo 'MaxStartups 1000:30:2000' >> /etc/ssh/sshd_config && \
    echo 'MaxSessions 1000' >> /etc/ssh/sshd_config && \
    echo 'LoginGraceTime 30' >> /etc/ssh/sshd_config && \
    echo 'ClientAliveInterval 30' >> /etc/ssh/sshd_config && \
    echo 'ClientAliveCountMax 3' >> /etc/ssh/sshd_config && \
    echo 'UseDNS no' >> /etc/ssh/sshd_config && \
    echo 'MaxAuthTries 10' >> /etc/ssh/sshd_config

# Create test files directory and generate test files
RUN mkdir -p /test-files && \
    echo "Small test file content" > /test-files/small.txt && \
    dd if=/dev/urandom of=/test-files/medium.bin bs=1024 count=1024 2>/dev/null && \
    dd if=/dev/urandom of=/test-files/large.dat bs=1024 count=10240 2>/dev/null && \
    chmod 644 /test-files/*

# Copy SSH keys
COPY test-keys/*.pub /tmp/keys/
RUN cat /tmp/keys/*.pub > /home/user/.ssh/authorized_keys 2>/dev/null || true && \
    chmod 600 /home/user/.ssh/authorized_keys && \
    chown user:user /home/user/.ssh/authorized_keys && \
    rm -rf /tmp/keys

EXPOSE 2222

# Start sshd in foreground
CMD ["/usr/sbin/sshd", "-D", "-e"]
```

### Memory Profile

![Memory Usage During Stress Test](memory_in_peak.png)

The memory profile shows stable behavior with no memory leaks. Memory peaked at ~320MB during the 2,000 parallel connections and was automatically reclaimed by .NET garbage collection afterward.

## See Also

- [Session Lifecycle](session-lifecycle.md) - Proper session management
- [Session Timeouts](session-timeouts.md) - Configuring timeouts
- [Keeping Connection Alive](keeping-connection-alive.md) - Keepalive configuration
- [Error Handling](error-handling.md) - Handling errors gracefully
