using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] usersFile = System.IO.File.ReadAllLines(@"C:\user.txt");

                //System.Console.WriteLine("Contents of user.txt = ");
                string[] arrFollows = { };
                //load user and who they follow
                List<UserInfo> lstUsers = GetTwitterUsers(usersFile, ref arrFollows);

                //get list of all follows
                List<Follows> lstFollows = GetTwitterFollows(usersFile, ref arrFollows, lstUsers);

                //sort the users alphabetical
                lstUsers.Sort((t1, t2) => t1.UserName.CompareTo(t2.UserName));


                //load tweets for each user
                string[] arrTweetFile = System.IO.File.ReadAllLines(@"C:\tweet.txt");

                int tweetEntryNr = 1;
                List<UserTweet> lstUserTweets = GetTweets(arrTweetFile, ref tweetEntryNr);


                //display all records for each user
                DisplayUserAndTweets(lstUsers, lstFollows, lstUserTweets);

                // Keep the console window open in debug mode.
                Console.WriteLine("Press any key to exit.");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                // Keep the console window open in debug mode.
                Console.WriteLine("Press any key to exit.");
                System.Console.ReadKey();
            }
        }

        private static void DisplayUserAndTweets(List<UserInfo> lstUsers, List<Follows> lstFollows, List<UserTweet> lstUserTweets)
        {
            //get each user 
            foreach (UserInfo userList in lstUsers)
            {
                System.Console.WriteLine(userList.UserName);
                System.Console.WriteLine("");

                

                //get tweets of the user and their follows
                var userFollows = lstFollows.Where(t => t.UserName == userList.UserName).ToList();

                int x = 1;
                foreach (var follows in userFollows)
                {
                    if (x==1)//ist time get the users tweets and follow tweets
                    {
                        var tweets = lstUserTweets.Where(t => t.UserName == follows.UserFollows || t.UserName == userList.UserName).ToList();
                        foreach (var userTweets in tweets)
                        {
                            System.Console.WriteLine("\t @" + userTweets.UserName + ":" + userTweets.TweetEntry);
                            System.Console.WriteLine("");

                        }
                    }
                    else//2nd time get only the user follow tweets
                    {
                        var tweets = lstUserTweets.Where(t => t.UserName == follows.UserFollows).ToList();
                        foreach (var userTweets in tweets)
                        {
                            System.Console.WriteLine("\t @" + userTweets.UserName + ":" + userTweets.TweetEntry);
                            System.Console.WriteLine("");

                        }
                    }
                    x++;
         
                }
                if (userFollows.Count == 0)//diplay only the users tweets if they have no followers
                {
                    var tweets = lstUserTweets.Where(t => t.UserName == userList.UserName).ToList();
                    foreach (var userTweets in tweets)
                    {
                        System.Console.WriteLine("\t @" + userTweets.UserName + ":" + userTweets.TweetEntry);
                        System.Console.WriteLine("");

                    }
                }

            }
        }

        private static List<UserTweet> GetTweets(string[] arrTweetFile, ref int tweetEntryNr)
        {
            List<UserTweet> lstUserTweets = new List<UserTweet>();
            foreach (string tweet in arrTweetFile)
            {

                int startIndex = tweet.IndexOf(">");
                string userName = tweet.Substring(0, startIndex).Trim();
                string userTweet = tweet.Substring(startIndex + 1);


                lstUserTweets.Add(new UserTweet()
                {
                    UserName = userName,
                    TweetEntry = userTweet,
                    TweetEntryNr = tweetEntryNr
                });

                tweetEntryNr++;

            }
            return lstUserTweets;
        }

        private static List<Follows> GetTwitterFollows(string[] usersFile, ref string[] arrFollows, List<UserInfo> lstUsers)
        {
            List<Follows> lstFollows = new List<Follows>();
            foreach (string users in usersFile)
            {
                // Use a tab to indent each line of the file.
                //Console.WriteLine("\t" + users);
                char[] seperator = { ',' };
                int startIndex = users.IndexOf("follows");
          
                string userName = users.Substring(0, startIndex);

                // get each users follows
                string strFollows = users.Substring(startIndex + 8);
                arrFollows = strFollows.Split(seperator);
                foreach (string follows in arrFollows)
                {
                    //add follows to the user list
                    var findUser = lstUsers.Where(t => t.UserName.Contains(follows.Trim())).ToList();
                    if (findUser.Count == 0)
                    {
                        lstUsers.Add(new UserInfo()
                        {
                            UserName = follows.Trim()                  
                        });

                    }
                    //checking that a user has only one record per follow
                    //otherwise it gets adde twice which will display twice
                    var followsDuplicates = lstFollows.Where(t => t.UserName == userName.Trim() && t.UserFollows==follows.Trim()).ToList();
                    if (followsDuplicates.Count == 0)
                    {
                        lstFollows.Add(new Follows()
                        {
                            UserName = userName.Trim(),
                            UserFollows = follows.Trim()
                        });
                    }
                }
            }

            
            return lstFollows;
        }

        private static List<UserInfo> GetTwitterUsers(string[] usersFile, ref string[] arrFollows)
        {
            List<UserInfo> lstUsers = new List<UserInfo>();

            foreach (string users in usersFile)
            {
                // Use a tab to indent each line of the file.
                //Console.WriteLine("\t" + users);
                char[] seperator = { ',' };
                int startIndex = users.IndexOf("follows");
                //string[] strFollows = users.Substring(startIndex).Split(seperator[0]); 
                string userName = users.Substring(0, startIndex);

                // get each users follows
                string strFollows = users.Substring(startIndex + 8);
                arrFollows = strFollows.Split(seperator);

                //add users to the list that are on twitter
                //loop through each follows and add them as a user

                var findUser = lstUsers.Where(t => t.UserName == userName.Trim()).ToList();
                if (findUser.Count == 0)
                {
                    //foreach (string follows in arrFollows)
                    //{
                        // add the user to the list
                        lstUsers.Add(new UserInfo()
                        {
                            UserName = userName.Trim(),
                            //Follows = follows.Trim(),
                            TweetEntryDislayed = 0
                        });


                    //}
                }
            }

            
            return lstUsers;
        }

        
    }
}
