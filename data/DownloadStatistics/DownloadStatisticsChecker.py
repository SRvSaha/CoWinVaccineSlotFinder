#!/usr/bin/python

# Little script to display file download statistics for
# the published releases of a Github project.
#
# (c) Gareth Watts 2015 
#     <gareth@omnipotent.net>

# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#     * Redistributions of source code must retain the above copyright
#       notice, this list of conditions and the following disclaimer.
#     * Redistributions in binary form must reproduce the above copyright
#       notice, this list of conditions and the following disclaimer in the
#       documentation and/or other materials provided with the distribution.
#     * Neither the name of the <organization> nor the
#       names of its contributors may be used to endorse or promote products
#       derived from this software without specific prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
# DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
# ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

import collections
import json
import os
import sys 
import urllib.request, urllib.parse, urllib.error

def dump_stats(repo):
    url = 'https://api.github.com/repos/%s/releases?per_page=100' % repo
    try:
        f = urllib.request.urlopen(url)
    except IOError as e:
        print("Failed to open API: %s" % e, file=sys.stderr) 
        return

    if f.getcode() == 404:
        print("Repo not found", file=sys.stderr)
        return

    if f.getcode() != 200:
        print("Got %d error while using API" % f.getcode(), file=sys.stderr)
        return

    releases = json.load(f)

    if len(releases) == 0:
        print("No releases")
        return

    tag_totals = collections.defaultdict(int)
    total = 0 

    print("%-25s %-40s %s" % ("Tag", "Filename", "Downloads"))
    for release in releases:
        for asset in release['assets']:
            print("%-25s %-40s %d" % (release['tag_name'], asset['name'], asset['download_count']))
            total += asset['download_count']
            tag_totals[release['tag_name']] += asset['download_count']

    print()
    print("TOTAL DOWNLOADS: ", total)

if __name__ == '__main__':
    if len(sys.argv) != 2:
        print("Usage git-release-stats.py <owner>/<repo>", file=sys.stderr)
        sys.exit(1)

    repo = sys.argv[1]
    print("Release stats for github.com/" + repo)
    print()
    dump_stats(repo)