using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMgr
{
    class Program
    {
        static void Main(string[] args)
        {
            int appId = 1;
            if(args.Length>0)
            {
                switch(args[0].ToUpper())
                {
                    case "-DL":
                        getAllDocList(appId); // 1. Can automaticlly produce document list pdf 2.Can convert docs to pdf file 3. can set pdf info in order to be automaticlly assemble later 
                        break;
                    case "-TL":
                        getAllDocTrack(appId); // 1. Can automatically produce doc requirement to client 2. can automaticlly email to client 3. can record every doc requirement record
                        break;
                    case "-NQ":
                        getAllNotQualifiedDocs(appId);
                        break;
                    default:
                        Console.WriteLine("You entered invalid parameters");
                        break;

                }
            }
            

            

           

        }

        private static void getAllNotQualifiedDocs(int appId)
        {
            using(DocDataContext dd = new DocDataContext())
            {
                List<tblDocument> docs = dd.tblDocuments.Where(x => x.ApplicationId == appId).Select(x => x).ToList();

                foreach(tblDocument doc in docs)
                {
                    // list all document who doesn't include sub-document tracking history 
                    List<tblDocTrack> tracks = dd.tblDocTracks.Where(x => (x.DocumentId == doc.Id && x.DocStatus == "Received & Qualifiying" && x.SubDocumentId == null)).Select(x => x).ToList();
                    foreach(tblDocTrack track in tracks) Console.WriteLine("Requested on: {0}\t received on: {1}, Issues: {2}\t {3} status: {4}",track.RequstDate, track.ReceivedDate,doc.DocName, track.Issues, track.DocStatus);

                    if(dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x) != null)
                    {
                        // list all sub document tracking history
                        List<tblSubDocument> subdocs = dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x).ToList();
                        foreach(tblSubDocument subdoc in subdocs)
                        {
                            List<tblDocTrack> subtracks = dd.tblDocTracks.Where(x => (x.DocumentId == doc.Id && x.SubDocumentId == subdoc.Id && x.DocStatus == "Received & Qualifiying")).Select(x => x).ToList();
                            foreach(tblDocTrack subtrack in subtracks) Console.WriteLine("{0} received on: {1}, {2},it's in process of {3}", doc.DocName, subtrack.ReceivedDate, subtrack.Issues, subtrack.DocStatus);

                        }
                    }

                }



            }
        }

        //private static void getAllDocTrack(int appId)
        //{
        //    using(DocDataContext dd = new DocDataContext())
        //    {
        //        List<tblDocument> docs = dd.tblDocuments.Where(x => x.ApplicationId == appId).Select(x => x).ToList();

        //        foreach(tblDocument doc in docs)
        //        {
        //            List<tblDocTrack> tracks = dd.tblDocTracks.Where(x =>( x.DocumentId == doc.Id && x.SubDocumentId==null)).Select(x => x).ToList();
        //            foreach(tblDocTrack track in tracks)
        //            {
        //                setupColor(track);
        //                Console.WriteLine("{0} received on: {1}\t{2}\tIssues: {3} ", doc.DocName, track.ReceivedDate, track.DocStatus, track.Issues);
        //            }

        //            if(dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x) != null)
        //            {
        //                List<tblSubDocument> subdocs = dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x).ToList();
        //                foreach(tblSubDocument subdoc in subdocs)
        //                {
        //                    List<tblDocTrack> subtracks = dd.tblDocTracks.Where(x => (x.DocumentId == doc.Id && x.SubDocumentId == subdoc.Id)).Select(x => x).ToList();
        //                    foreach(tblDocTrack subtrack in subtracks)
        //                    {
        //                        // setupColor(subtrack);
        //                        Console.ForegroundColor = ConsoleColor.White;
        //                        Console.WriteLine("{0} received on: {1}\t{2}\tIssues:{3}", doc.DocName, subtrack.ReceivedDate, subtrack.DocStatus, subtrack.Issues);
        //                    }

        //                }
        //            }

        //        }



        //    }
        //}
        private static void getAllDocTrack(int appId)
        {
            using(DocDataContext dd = new DocDataContext())
            {
                List<tblDocTrack> tracks = dd.tblDocTracks.Where(x => x.ApplicationId == 1).Select(x => x).OrderBy(x=>x.DocStatus).ToList();
                foreach(tblDocTrack t in tracks) {

                   string dn= dd.tblDocuments.Where(x => x.Id == t.DocumentId).Select(x => x.DocName).FirstOrDefault();
                   string sdn = dd.tblSubDocuments.Where(x => x.Id == t.SubDocumentId).Select(x => x.DocName).FirstOrDefault();
                   setupColor(t);
                   if(t.SubDocumentId==null) Console.WriteLine("{0} received on: {1}\t{2}\tIssues:{3}", dn, t.ReceivedDate, t.DocStatus, t.Issues);
                   else Console.WriteLine("\t{0} received on: {1}\t{2}\tIssues:{3}", sdn, t.ReceivedDate, t.DocStatus, t.Issues);

                }
            }
        }

        private static void setupColor(tblDocTrack track)
        {
            switch(track.DocStatus)
            {
                case "Waitting":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Qualified":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "Received & Qualifiying":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }
        }

        private static void getAllDocList(int appId)
        {
            using(DocDataContext dd = new DocDataContext())
            {

                
                List<tblDocument> docs = dd.tblDocuments.Where(x => x.ApplicationId == appId).Select(x => x).ToList();
                foreach(tblDocument doc in docs)
                {
                    Console.WriteLine("Doc Number: {0}\tDoc Name: {1}\tDoc owner: {2}\tRemark: {3}", doc.DocNumber, doc.DocName, doc.DocOwner, doc.Remark);
                    if(dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x) != null)
                    {
                        List<tblSubDocument> subdocs = dd.tblSubDocuments.Where(x => x.DocumentId == doc.Id).Select(x => x).ToList();
                        foreach(tblSubDocument subdoc in subdocs)
                            Console.WriteLine("\tSub Doc Id: {0}\tDoc Name: {1}\tRemark: {2}", subdoc.Id, subdoc.DocName, subdoc.Remark);
                    }

                }
                Console.WriteLine();

            }
        }
    }
}
