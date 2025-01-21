#!/bin/bash
# description: Persistence

start() {
    /opt/persistence
}

stop() {
    /usr/bin/true
}

case "$1" in 
    start)
       start
       ;;
    stop)
       stop
       ;;
    restart)
       stop
       start
       ;;
    status)
       ;;
    *)
       echo "Usage: $0 {start|stop|status|restart}"
esac
