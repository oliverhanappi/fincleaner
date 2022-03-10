# FinCleaner

FinCleaner is a simple tool to prepare transactions exported from
your bank account for analyzation or import into another tool like
Firefly III.

It features a simple rule engine to parse, filter and modify CSV
files. It is possible to modify fields and remove transactions.

Furthermore, FinCleaner generates warnings for duplicate transactions,
e.g. for transfers between several accounts.

## Usage

Create a configuration file appropriate for your bank exports.
A single configuration file may contain multiple parser definitions
and rule sets, which are chosen automatically based on file names.

Then invoke FinCleaner with the configuration file and your exported
CSV files, e.g. by drag & dropping all files onto the application.

## Documentation

There is currently no documentation at the moment.