#!/usr/bin/env osascript -l JavaScript

ObjC.import("Foundation");

var task = $.NSTask.alloc.init;
task.launchPath = "/usr/bin/open";
task.arguments = ["/System/Applications/Calculator.app"];
task.launch;
task.waitUntilExit;

console.log("success");