import datetime
import os
import sys

from constgen.data import ConstClass, ConstMember
from constgen.source import Item, Folder, File
from mako.template import Template

"""
CONSTANTS
"""

IGNORED_FILE_PATH_KEYWORD = [
    'Prefabs',
	'Packages',
    'Scenes',
	'Sounds',
	'Videos',
	'Visuals'
]

"""
FACTORIES
"""


def createConstClass(name, members):
    return ConstClass(name, members)


def createConstMembers(items):
    return list(map(lambda item: ConstMember(item.value, item.path), sorted(set(items))))


"""
MAIN PROGRAM
"""


def determine_if_path_is_in_ignored_files(path):
    is_path_in_ignored_files = False
    for ignored_file_path_keyword in IGNORED_FILE_PATH_KEYWORD:
        if ignored_file_path_keyword in path:
            is_path_in_ignored_files = True

    return is_path_in_ignored_files


if len(sys.argv) != 3 or "-h" in sys.argv:
    print("""Generate a class of usefull consts using a Unity Project. Generates consts for Layers, Tags, Scenes, Prefabs, GameObjects and Animator Parameters.

             Usage :            
                 HarmonyCodeGenerator.py inputDir outputDir
             Where :
                 inputDir                Path to the Unity project directory (not the Asset folder).
                 outputDir               Path to the output directory for generated code (in the Asset folder).""")
else:

    # Arguments
    projectDirectoryPath = sys.argv[1]
    outputDirectoryPath = sys.argv[2]

    print("input dir: {}".format(sys.argv[1]))
    print("output dir: {}".format(sys.argv[2]))

    print("Creating (if not exist) output folder.")

    # Make directories
    if not os.path.exists(outputDirectoryPath):
        os.makedirs(outputDirectoryPath)

    # Template
    constClasses = []
    template = Template(filename=os.path.join(os.path.dirname(os.path.realpath(__file__)), "R.cs.mak"))

    # Layers
    print("    Layers...")
    tagManagerFilePath = os.path.join(projectDirectoryPath, "ProjectSettings", "TagManager.asset")
    layers = File(tagManagerFilePath, "/0/TagManager/layers/*").readConsts()
    layers.append(Item('None', ''))
    constClasses.append(createConstClass("Layer", createConstMembers(layers)))

    # Tags
    print("    Tags...")
    tags = File(tagManagerFilePath, "/0/TagManager/tags/*").readConsts()
    tags.append(Item('None', tagManagerFilePath))
    tags.append(Item('Untagged', tagManagerFilePath))
    tags.append(Item('Respawn', tagManagerFilePath))
    tags.append(Item('Finish', tagManagerFilePath))
    tags.append(Item('EditorOnly', tagManagerFilePath))
    tags.append(Item('Player', tagManagerFilePath))
    tags.append(Item('MainCamera', tagManagerFilePath))
    constClasses.append(createConstClass("Tag", createConstMembers(tags)))

    # Scenes
    print("    Scenes...")
    scenes = Folder(os.path.join(projectDirectoryPath, "Assets"), ".unity").readConsts()
    scenes.append(Item('None', ''))
    constClasses.append(createConstClass("Scene", createConstMembers(scenes)))

    # Prefabs
    print("    Prefabs...")
    prefabs = Folder(os.path.join(projectDirectoryPath, "Assets"), ".prefab").readConsts()
    prefabs.append(Item('None', ''))
    constClasses.append(createConstClass("Prefab", createConstMembers(prefabs)))

    # GameObjects
    print("    GameObjects...")
    gameObjects = [Item('None', '')]
    for item in scenes:
        if item.path != '' and not determine_if_path_is_in_ignored_files(item.path):
            gameObjects += File(item.path, "/*/GameObject/m_Name").readConsts()
    for item in prefabs:
        if item.path != '' and not determine_if_path_is_in_ignored_files(item.path):
            gameObjects += File(item.path, "/*/GameObject/m_Name").readConsts()
    constClasses.append(createConstClass("GameObject", createConstMembers(gameObjects)))

    # Animator Parameters
    print("    AnimationParameters...")
    animatorParameters = [Item('None', '')]
    for item in Folder(os.path.join(projectDirectoryPath, "Assets"), ".controller").readConsts():
        if item.path != '':
            animatorParameters += File(item.path, "/*/AnimatorController/m_AnimatorParameters/*/m_Name").readConsts()
    constClasses.append(createConstClass("AnimatorParameter", createConstMembers(animatorParameters)))

    # Generate
    print("")
    print("Generating Const Classes.")
    with open(os.path.join(outputDirectoryPath, "R.cs"), "w") as file:
        print("Writing Const Classes.")
        file.write(template.render(timeStamp=datetime.datetime.utcnow(), constClasses=constClasses))

    print("                                                         ..\n"
          "                                         .    ....▄▀  ▀▄.\n"
          "                                       #█▄ .æ╙   └       ╙▀▄\n"
          "                                      ╫       ,    ▄▄       ▄\n"
          "                                     ╫⌐   ▄╜   º'  ▀█,      ▀▄\n"
          "                                    ╒▀ ▓'▄           ╙'' ╙█▄ ▐▌\n"
          "                                      ▌▀            ▀      ▐▀  █\n"
          "              ,⌐⌐                    ╓▀  ▓    ,          .▀▄   █\n"
          "            ╩    ▌                   █   ▀  .╩    ▀█▌    ┌╙▄¥ ▐▌\n"
          "           █    ▐                   ╫      ▓Ö            ⁿ▄╦φ▓█\n"
          "           █    b                   █      ╙▀            ¥  █▀\n"
          "           ┌▄   ▀                   █⌐(█∞⌐.   .,»▀▄         █⌐\n"
          "      ╓▀     ╙%▄  ▀,                └█  '½⌐.  .╓═'▀        .█\n"
          "     ▀         █  ▀▄,               ▀▌   -▄█           ▓▀▀\n"
          "      ╓▀  ""╙%▄▄█   ██▀▀▓╗▄⌐.         ▀█             ┌▓█ ⁿ\n"
          "      ▀         ▓  # █│││││ÑÑÑÑ▀▀▀▀▀ÑÑÑÑ█%▄        º▀█  ╙▓▄\n"
          "       █  ""╙%▄▄▀ ▐▌ █M││││││││││││││││╫   ▀█▓=^ .▄#▀   ▄█│Ñ▒╗,\n"
          "       █.      █b ▀ ▄▒║▒▒#N││││││││││││Ü█     ╙╙└     .▓▀│││││Ñ▓▄\n"
          "        ╙▀█▄▒'▀▄æ'╓▓Ñ║║║║║║║▒▒▒▒#▄W│││││║▌         ▄æ▓▀∩││││││││Å▀▄\n"
          "              ╙º▀▀█▓▓▓▓▓▓▓▓▓▀▀▀╙╙█││││││█      ▓▀▀Ñ∩││││││││││││││Å▀▄\n"
          "                                ╒▌│││││╫      ▐▌││││││││││││││││││││Ñ▓\n"
          "                                ╫▌│││││█      █│││││││││││││││││││││││▀▄\n"
          "                                █MÑÑÑÑ║▀     ▐▌ÑÑÑÑÑÑÑÑÑ▒▓▄│ÑÑÑÑÑÑÑÑÑÑÑÑ▄\n"
          "")
