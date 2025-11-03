FROM alpine:latest

# Install OpenSSH server and utilities needed for tests
RUN apk add --no-cache openssh-server openssh-keygen openssh-client ncurses coreutils

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
    chmod 644 /test-files/* && \
    chmod 755 /test-files

# Setup /tmp directory with proper permissions for the user
RUN mkdir -p /tmp && \
    chmod 1777 /tmp && \
    chown user:user /home/user && \
    chmod 755 /home/user

# Copy SSH keys
COPY test-keys/*.pub /tmp/keys/
RUN cat /tmp/keys/*.pub > /home/user/.ssh/authorized_keys 2>/dev/null || true && \
    chmod 600 /home/user/.ssh/authorized_keys && \
    chown user:user /home/user/.ssh/authorized_keys && \
    rm -rf /tmp/keys

EXPOSE 2222

# Start sshd in foreground
CMD ["/usr/sbin/sshd", "-D", "-e"]
