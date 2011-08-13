# Run from Git bash shell window not just from cmd.exe:
# $ sh make-src-package.sh
mkdir -p src
git archive --format zip --output src/xrouter-src_`date +%Y-%m-%d_%H-%M-%S`.zip develop