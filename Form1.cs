using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Data.Entity.Validation;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Net.Http;



namespace ApolloReferences
{
    public partial class WrapHtmlLinks : Form
    {
        WrapHtml entity = new WrapHtml();
        Ref_Links grl = new Ref_Links();
		
        private bool isBrokenLink(string url)
        {

            Boolean isBrokenLink = false;
            try
            {
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
                http.UserAgent = "Mozilla/9.0 (compatible; MSIE 6.0; Windows 98)";
                http.Method = "HEAD";
                http.Timeout = 1000000;
                http.AllowAutoRedirect = true;
           
                try
                {
                    isBrokenLink = false;
                    grl.Content = RefLinkHtml;

                 
                }
                catch (Exception ex)
                {

                    isBrokenLink = true;

                }


            }
            catch (Exception ex)
            {
                isBrokenLink = true;

            }
           
            finally
            {
				//It allows us to combine several operations to be combined within the same transaction 
				//and hence all the transactions are either committed or rolled back

                using (var transaction = entity.Database.BeginTransaction())
                {
					
                    entity.Links_Content.Add(grl);
                    entity.SaveChanges();
                    transaction.Commit();

                }

            }

            return isBrokenLink;

        }

        private async void Link_Click_Cmd(object sender, EventArgs e)
        {
            MessageBox.Show("Processed Content Generation");
			
          
            var allLinks = entity.RefLinks.ToList();

            foreach (var link in allLinks)
            {
                if (url!= null)
                {
                    try
                    {
                        //Avoid pdf link
                        if (url.Split('.').Last().ToLower() != "pdf")
                        {
     
	 
						/*----------------------------------------------------------------------------------------------------------
							WebClient client = new WebClient()
							string wrapthtmlString =  client.DownloadStringAsync(url);//await client.DownloadTaskStringAsync(url)
							//It will take to download 8 ms
							
							But best way to use to better performance
							HttpClient it will take 3 ms

						------------------------------------------------------------------------------------------------------------*/
	 

                            using (HttpClient HttpClient = new HttpClient())
                            using (HttpResponseMessage response = await HttpClient.GetAsync(link.Link))
                            using (HttpContent content = response.Content)
                            {

                                try
                                {	
									//Download HTML string form Link 
                                    string result = await content.ReadAsStringAsync();
                                    isBrokenLink(url);
                                }
                                catch (WebException ex)
                                {
                                    Console.WriteLine("This program is expected to throw WebException on successful run." +
                                                        "\n\nException Message :" + ex.Message);
                                    if (ex.Status == WebExceptionStatus.ProtocolError)
                                    {
                                        Console.WriteLine("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
                                        Console.WriteLine("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);

                                }

                            }
                            
                        }
                        else
                        {
                            //If pdf link default storing null
                            RefLinkHtml = null;
                            isBrokenLink(url);
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine("This program is expected to throw WebException on successful run." +
                                            "\n\nException Message :" + ex.Message);
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            Console.WriteLine("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
                            Console.WriteLine("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }
        }

    }
}
