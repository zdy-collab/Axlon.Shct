#!/bin/sh

max_attempts="${CLONE_RETRY_ATTEMPTS:-8}"
retry_delay="${CLONE_RETRY_DELAY_SECONDS:-5}"
plugin_bin="${CLONE_PLUGIN_BIN:-/bin/plugin-git}"
attempt=1

while true; do
  echo "clone attempt ${attempt}/${max_attempts}"
  "${plugin_bin}" "$@"
  status=$?

  if [ "${status}" -eq 0 ]; then
    exit 0
  fi

  if [ "${attempt}" -ge "${max_attempts}" ]; then
    echo "clone failed after ${max_attempts} attempts"
    exit "${status}"
  fi

  delay=$((attempt * retry_delay))
  echo "clone failed with exit status ${status}; retrying in ${delay}s"
  sleep "${delay}"
  attempt=$((attempt + 1))
done
