#!/bin/sh
# SSH Agent setup script

echo "=== SSH Agent Setup ==="

# Install openssh (correct package name for Alpine)
apk add --no-cache openssh

# Create directory for socket
mkdir -p /ssh-agent
chmod 755 /ssh-agent

# Remove old socket if exists
rm -f /ssh-agent/socket

# Start ssh-agent
eval $(ssh-agent -s -a /ssh-agent/socket)

echo "SSH_AUTH_SOCK=/ssh-agent/socket"
echo "SSH_AGENT_PID=$SSH_AGENT_PID"

# Copy keys to /tmp and fix permissions (mounted keys are read-only)
mkdir -p /tmp/keys
if [ -f /keys/id_rsa ]; then
    cp /keys/id_rsa /tmp/keys/id_rsa
    chmod 600 /tmp/keys/id_rsa
    ssh-add /tmp/keys/id_rsa 2>/dev/null && echo "Added id_rsa to agent" || echo "Failed to add id_rsa"
fi

if [ -f /keys/id_ed25519 ]; then
    cp /keys/id_ed25519 /tmp/keys/id_ed25519
    chmod 600 /tmp/keys/id_ed25519
    ssh-add /tmp/keys/id_ed25519 2>/dev/null && echo "Added id_ed25519 to agent" || echo "Failed to add id_ed25519"
fi

# List loaded identities
echo "Loaded identities:"
ssh-add -l

echo "=== SSH Agent Ready ==="

# Keep container running (agent must stay alive)
tail -f /dev/null
