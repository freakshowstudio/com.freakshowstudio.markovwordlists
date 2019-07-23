
# Markov Word Lists

This package lets you import word list files into Unity and use them for
example for procedural name generation.

The word list file should be a plain text file containing each word on a
separate line, and should be named .markov

Currently all imported words are converted to lowercase. An option to preserve
case might be added in the future.

You can set the desired order for each file, which is how many characters the
chain "looks back". A higher value gives more believable results, but are
less random. A higher value will also decrease performance and increase
memory usage. Values in the range 1-3 generally work best.

For each file, you also need to specify a start and end character. These are
used internally for keeping track of the start and end words. You can choose
whichever symbol you want for this, but it should not appear in your
wordlist file.

See the samples for usage examples.
