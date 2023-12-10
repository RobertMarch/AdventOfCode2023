echo "Setting up day $1"

DAY_NUMBER=`printf "%02d" $1`
FILE_NAME="day${DAY_NUMBER}.cs"

if test -f "$FILE_NAME"; then
    echo "File already exists"
    exit 1
fi

cp src/Days/dayXX.cs "src/Days/${FILE_NAME}"
sed -i 's/0/'"$1"'/' src/Days/$FILE_NAME
sed -i 's/XX/'"$DAY_NUMBER"'/g' "src/Days/${FILE_NAME}"

touch "inputs/day$DAY_NUMBER.txt"
sed -i 's_// New days here_{ '"$DAY_NUMBER"', new Day'"$DAY_NUMBER"'() },\n    // New days here_' Program.cs
sed -i 's/dayNumber = [0-9]\+;/dayNumber = '"$1"';/' Program.cs

echo "Set up for day $1 complete"
