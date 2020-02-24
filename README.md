Behavioral code analysis toolkit
===

A collection of scripts in varying programming languages to get started on behavioral code analysis.
Full credit for these techniques goes to [Adam Tornhill](https://www.adamtornhill.com/).

Getting started
--

* Gather complexity information of your codebase using [cloc](https://github.com/AlDanial/cloc): `cloc --by-file --csv "." > cloc.csv`
* Extract your version control log data as documented on [code-maat](https://github.com/adamtornhill/code-maat#generating-input-data): `git log --all --numstat --date=short --pretty=format:"--%h--%ad--%aN" --no-renames --after=2019-01-01`
* Feed version control output to code-maat: `//λ java -jar code-maat-1.1-SNAPSHOT-standalone.jar -c git2 -l "git.log" > codemaat-results.csv`
* You can use the provided F# scripts to generate charts & d3js data. A good place to start is hotspot analysis in [hotspot.fsx](https://github.com/jovaneyck/behavioral-code-analysis-toolkit/blob/master/fsharp/scripts/hotspots.fsx).
* You can render generated d3js data using the [d3js html](https://github.com/jovaneyck/behavioral-code-analysis-toolkit/blob/master/dataviz-d3js/index.html).

Example output
---
* [hotspot analysis](https://jovaneyck.github.io/mithra-dataviz)

Further reading
---

* [codescene.io](https://codescene.io/), behavioral-code-analysis-as-a-service
* [Your code as a crime scene, Tornhill](https://pragprog.com/book/atcrime/your-code-as-a-crime-scene/)
* [Software design X-rays, Tornhill](https://pragprog.com/book/atevol/software-design-x-rays)
* [Lightning talk](https://docs.google.com/presentation/d/1zZrppPp6_20r4g1lC0pqrOvw03oi3e52zJbF-Hw8Yu8/edit?usp=sharing)
* [Workshop](https://drive.google.com/file/d/1EpIXiDEqalVaerotQ5e2BcmHwn6zStMI/view?usp=sharing)
