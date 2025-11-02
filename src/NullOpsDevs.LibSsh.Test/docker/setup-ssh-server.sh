#!/bin/bash
# SSH Server setup script for testing
# This script runs during container initialization

echo "=== SSH Test Server Setup ==="

# Convert all key files to Unix line endings (in case they have Windows CRLF)
echo "Converting key files to Unix line endings..."
if command -v dos2unix >/dev/null 2>&1; then
    find /keys -type f -exec dos2unix {} \; 2>/dev/null || true
else
    echo "dos2unix not available, skipping line ending conversion"
fi

# Create test files directory if it doesn't exist
mkdir -p /test-files

# Create test files with known content
echo "Small test file content" > /test-files/small.txt
dd if=/dev/urandom of=/test-files/medium.bin bs=1024 count=1024 2>/dev/null  # 1MB
dd if=/dev/urandom of=/test-files/large.dat bs=1024 count=10240 2>/dev/null # 10MB

# Set proper permissions
chmod 644 /test-files/small.txt
chmod 644 /test-files/medium.bin
chmod 644 /test-files/large.dat

# Setup authorized_keys for public key auth
mkdir -p /config/.ssh
chmod 700 /config/.ssh

if [ -f /keys/id_rsa.pub ]; then
    cat /keys/id_rsa.pub >> /config/.ssh/authorized_keys
    echo "Added id_rsa.pub to authorized_keys"
fi

if [ -f /keys/id_rsa_protected.pub ]; then
    cat /keys/id_rsa_protected.pub >> /config/.ssh/authorized_keys
    echo "Added id_rsa_protected.pub to authorized_keys"
fi

if [ -f /keys/id_ed25519.pub ]; then
    cat /keys/id_ed25519.pub >> /config/.ssh/authorized_keys
    echo "Added id_ed25519.pub to authorized_keys"
fi

chmod 600 /config/.ssh/authorized_keys
chown -R 1000:1000 /config/.ssh

# Setup host-based authentication
mkdir -p /etc/ssh
echo "ssh-test" > /etc/ssh/shosts.equiv
echo "localhost" >> /etc/ssh/shosts.equiv
chmod 644 /etc/ssh/shosts.equiv

echo "=== SSH Test Server Setup Complete ==="
