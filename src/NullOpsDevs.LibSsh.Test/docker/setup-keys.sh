#!/bin/sh
# Setup authorized keys and test files

echo "Setting up SSH keys..."

# Setup authorized_keys
if [ -d /keys ]; then
    cat /keys/*.pub > /home/user/.ssh/authorized_keys 2>/dev/null || true
    chmod 600 /home/user/.ssh/authorized_keys
    chown user:user /home/user/.ssh/authorized_keys
    echo "Authorized keys configured"
fi

# Create test files
echo "Creating test files..."
echo "Small test file content" > /test-files/small.txt
dd if=/dev/urandom of=/test-files/medium.bin bs=1024 count=1024 2>/dev/null
dd if=/dev/urandom of=/test-files/large.dat bs=1024 count=10240 2>/dev/null
chmod 644 /test-files/*
echo "Test files created"

echo "Setup complete, starting SSHD..."
