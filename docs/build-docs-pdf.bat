rem --- convert LaTeX files to PDF ---
rem assumes installed LaTeX, eg. in TeXLive

cd DaemonNT\latex
pdflatex refman.tex
makeindex refman.idx
pdflatex refman.tex
rem run pdfalatex again to get cross-references right
pdflatex refman.tex
cp refman.pdf ..\DaemonNT-API-docs.pdf

cd ..\..\SchemaTron\latex
pdflatex refman.tex
makeindex refman.idx
pdflatex refman.tex
pdflatex refman.tex
cp refman.pdf ..\SchemaTron-API-docs.pdf

cd ..\..\XRouter\latex
pdflatex refman.tex
makeindex refman.idx
pdflatex refman.tex
pdflatex refman.tex
cp refman.pdf ..\XRouter-API-docs.pdf

cd ..\..
