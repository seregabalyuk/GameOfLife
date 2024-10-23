import re
import PIL
import PIL.Image

filename = input()
file = open(filename, "r")
x = 0
y = 0
bigs = ""

for s in file:
    if re.match("(\s*\w+\s*=\s*\S*,)*\s*\w+\s*=\s*\S*\s*$", s):
        vars = re.split("\s*,\s*", s)
        for i1 in range(2):
            exec(vars[i1])
        image = PIL.Image.new(mode="RGB", size=(x, y))
        pixels = image.load()
    if re.match("(\d|b|o|\$)+!?(\s)*$", s):
        bigs += s.split()[0]
num = 0
i = 0
j = 0

for c in bigs:
    if re.match("\d", c):
        num = num * 10 + int(c)
    elif re.match("b|o|\$", c):
        povt = 1 if num == 0 else num
        num = 0
        if c == 'o':
            for _ in range(povt):
                pixels[i, j] = (255, 255, 0)
                i += 1
        elif c == 'b':
            i += povt
        elif c == '$':
            i = 0
            j += povt
image.show()
newfilename = ".".join(filename.split(".")[:-1]) + ".png"
image.save(newfilename)

