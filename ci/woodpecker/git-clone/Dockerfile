FROM woodpeckerci/plugin-git:2.9.2

COPY --chmod=755 retry-clone.sh /usr/local/bin/retry-clone

ENTRYPOINT ["/usr/local/bin/retry-clone"]
