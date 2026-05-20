using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace RosaryScroll
{
    public sealed class MysteryGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public string SetName { get; set; }
        public List<RosaryImage> Images { get; set; }
        public List<RosaryImage> BrowseImages { get; set; }

        private Visibility _controlsVisibility = Visibility.Collapsed;
        public Visibility ControlsVisibility
        {
            get { return _controlsVisibility; }
            set
            {
                if (_controlsVisibility != value)
                {
                    _controlsVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlsVisibility)));
                }
            }
        }

        private int _activeImageIndex = 0;
        public int ActiveImageIndex
        {
            get { return _activeImageIndex; }
            set
            {
                if (_activeImageIndex != value)
                {
                    _activeImageIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveImageIndex)));
                }
            }
        }
    }

    public sealed class RosaryImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string MysteryName { get; set; }
        public string PrayerLabel { get; set; }
        public string ImageUri { get; set; }
        public BitmapImage ImageSource { get; set; }
        public int DecadePrayerNumber { get; set; }

        private Visibility _controlsVisibility = Visibility.Collapsed;
        public Visibility ControlsVisibility
        {
            get { return _controlsVisibility; }
            set
            {
                if (_controlsVisibility != value)
                {
                    _controlsVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControlsVisibility)));
                }
            }
        }
    }

    public static class RosaryData
    {
        public static List<MysteryGroup> CreateMysteries()
        {
            List<MysteryGroup> mysteries = new List<MysteryGroup>();

            AddMystery(mysteries, "Joyful", "The Annunciation", "joyful-annunciation");
            AddMystery(mysteries, "Joyful", "The Visitation", "joyful-visitation");
            AddMystery(mysteries, "Joyful", "The Nativity", "joyful-nativity");
            AddMystery(mysteries, "Joyful", "The Presentation", "joyful-presentation");
            AddMystery(mysteries, "Joyful", "The Finding in the Temple", "joyful-finding-temple");

            AddMystery(mysteries, "Luminous", "The Baptism in the Jordan", "luminous-baptism");
            AddMystery(mysteries, "Luminous", "The Wedding at Cana", "luminous-cana");
            AddMystery(mysteries, "Luminous", "The Proclamation of the Kingdom", "luminous-proclamation");
            AddMystery(mysteries, "Luminous", "The Transfiguration", "luminous-transfiguration");
            AddMystery(mysteries, "Luminous", "The Institution of the Eucharist", "luminous-eucharist");

            AddMystery(mysteries, "Sorrowful", "The Agony in the Garden", "sorrowful-agony");
            AddMystery(mysteries, "Sorrowful", "The Scourging at the Pillar", "sorrowful-scourging");
            AddMystery(mysteries, "Sorrowful", "The Crowning with Thorns", "sorrowful-crowning");
            AddMystery(mysteries, "Sorrowful", "The Carrying of the Cross", "sorrowful-carrying-cross");
            AddMystery(mysteries, "Sorrowful", "The Crucifixion", "sorrowful-crucifixion");

            AddMystery(mysteries, "Glorious", "The Resurrection", "glorious-resurrection");
            AddMystery(mysteries, "Glorious", "The Ascension", "glorious-ascension");
            AddMystery(mysteries, "Glorious", "The Descent of the Holy Spirit", "glorious-descent");
            AddMystery(mysteries, "Glorious", "The Assumption", "glorious-assumption");
            AddMystery(mysteries, "Glorious", "The Coronation", "glorious-coronation");

            return mysteries;
        }

        public static List<RosaryImage> CreatePrayerSlides(List<MysteryGroup> mysteries, string setName)
        {
            List<RosaryImage> slides = new List<RosaryImage>();

            for (int index = 0; index < mysteries.Count; index++)
            {
                MysteryGroup mystery = mysteries[index];
                if (mystery.SetName != setName)
                {
                    continue;
                }

                int availableCount = mystery.Images.Count;
                for (int bead = 0; bead < 10; bead++)
                {
                    var baseImage = mystery.Images[bead % availableCount];
                    slides.Add(new RosaryImage
                    {
                        MysteryName = mystery.Name,
                        PrayerLabel = setName + " Mystery - Hail Mary " + (bead + 1) + " of 10",
                        ImageUri = baseImage.ImageUri,
                        ImageSource = baseImage.ImageSource,
                        DecadePrayerNumber = bead + 1
                    });
                }
            }

            return slides;
        }

        private static void AddMystery(List<MysteryGroup> mysteries, string setName, string name, string filePrefix)
        {
            MysteryGroup mystery = new MysteryGroup
            {
                Name = name,
                SetName = setName,
                Images = new List<RosaryImage>(),
                BrowseImages = new List<RosaryImage>()
            };

            string installPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            string mysteriesFolder = Path.Combine(installPath, "Assets", "Mysteries");
            if (!Directory.Exists(mysteriesFolder))
            {
                mysteriesFolder = Path.Combine(installPath, "RosaryScroll", "Assets", "Mysteries");
            }

            for (int bead = 1; bead <= 30; bead++)
            {
                string filename = filePrefix + "-" + bead.ToString("00") + ".jpg";
                string fullPath = Path.Combine(mysteriesFolder, filename);
                if (File.Exists(fullPath))
                {
                    string imageUri = "ms-appx:///Assets/Mysteries/" + filename;
                    mystery.Images.Add(new RosaryImage
                    {
                        MysteryName = name,
                        PrayerLabel = setName + " Mystery - Artwork " + bead,
                        ImageUri = imageUri,
                        ImageSource = new BitmapImage(new Uri(imageUri)),
                        DecadePrayerNumber = bead
                    });
                }
            }

            if (mystery.Images.Count == 0)
            {
                string fallbackUri = "ms-appx:///Assets/Mysteries/" + filePrefix + "-01.jpg";
                mystery.Images.Add(new RosaryImage
                {
                    MysteryName = name,
                    PrayerLabel = setName + " Mystery - Artwork 1",
                    ImageUri = fallbackUri,
                    ImageSource = new BitmapImage(new Uri(fallbackUri)),
                    DecadePrayerNumber = 1
                });
            }

            mystery.BrowseImages.AddRange(mystery.Images);
            if (mystery.Images.Count > 1)
            {
                mystery.BrowseImages.Add(mystery.Images[0]);
            }

            mysteries.Add(mystery);
        }
    }
}
