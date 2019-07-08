#!/usr/bin/env python
import copy
import sys
import xml.etree.ElementTree as ET
import json

ET.register_namespace('', "http://www.w3.org/2000/svg")
ET.register_namespace('xlink', "http://www.w3.org/1999/xlink")

tree = ET.parse(sys.argv[1])
root = tree.getroot()
# print(root)
listofgroup = []
listofcolor = []
count = 0
for g in root.findall('{http://www.w3.org/2000/svg}path'):
    # name = g.get('id')
    color = g.get('fill')
    g.set('fill', "#FFFFFF")
    count += 1
    listofgroup.append(count)
    listofcolor.append(color)
print(listofgroup)

# write config json file
json = json.dumps(listofcolor)
with open(sys.argv[1].replace('.svg', '_config'),'w') as f:
    f.write(json)

for counter in range(len(listofgroup)):
    temp = listofgroup[:]
    temp_tree = copy.deepcopy(tree)
    del temp[counter]
    print(temp)
    temp_root = temp_tree.getroot()
    temp_count = 0
    for g in temp_root.findall('{http://www.w3.org/2000/svg}path'):
        # name = g.get('id')
        temp_count += 1
        if temp_count in temp:
            temp_root.remove(g)
    temp_tree.write(sys.argv[1].replace('.svg', '_' + str(listofgroup[counter]) + '.svg'))
   
