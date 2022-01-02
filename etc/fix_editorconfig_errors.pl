#! /usr/bin/env perl

use strict;
use warnings;
use v5.20;

#
# To use this, run `dotnet build | etc/fix_readonly_errors.pl` to capture the output.
#

my %by_file;

while( <> ) {
    my( $path, $line, $col, $err, $msg ) = / ^ (.*?) \( (\d+) , (\d+) \) : \s error \s (.{7}) : (.*?) \s \[ /x;
    
    push @{$by_file{$path}}, { line=>$line-1, col=>$col, err=>$err, msg=>$msg } if $path && $line && $col && $err && $msg;
}

my %done;

foreach my $file ( sort keys %by_file ) {
    say STDERR "Processing $file...";
    open my $in, '<', $file or die "$!\n";
    chomp( my @lines = <$in> );
    close $in;

    say "$file:";
    foreach my $issue ( sort { $a->{line} <=> $b->{line} } @{$by_file{$file}} ) {
        my $line = $lines[$issue->{line}];
        my $err = $issue->{err};
        my $lineno = $issue->{line};
        my $col = $issue->{col};

        next if $done{join(":", $file, $lineno, $col)};
        $line = fix_ide0044( $line ) if $err eq 'IDE0044';
        $line = fix_ide0005( $line ) if $err eq 'IDE0005';

        $lines[$issue->{line}] = $line;
        $done{join(":", $file, $lineno, $col)} = 1;
    }

    open my $out, '>', $file or die "$!\n";
    foreach( @lines ) {
        say $out $_ unless /\<REDACTED\>/;
    }
    close $out;
}

sub fix_ide0044 {
    my $line = shift @_;

    say "  IDE0044";
    say "  |$line";
    my( $ws, $rest ) = $line =~ /^(\s+)(.*)$/;
    my $fixed = $ws . 'readonly ' . $rest;
    say "  |$fixed";
    return $fixed;
}

sub fix_ide0005 {
    my $line = shift @_;
    say "  IDE0005";
    say "  |$line";
    return '<REDACTED>';
}